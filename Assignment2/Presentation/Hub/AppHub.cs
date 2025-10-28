using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class AppHub : Hub
{
    private readonly UserConnectionStore _store;
    public AppHub(UserConnectionStore store) => _store = store;

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleValue = user.FindFirstValue(ClaimTypes.Role); // ví dụ "1", "3", v.v.

            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

            // 🧠 Role là số: 1 = Staff, 3 = Admin (theo database của bạn)
            if (roleValue == "1")
                await Groups.AddToGroupAsync(Context.ConnectionId, "staff");
            else if (roleValue == "3")
                await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var uid = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(uid))
        {
            _store.Remove(uid, Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(ex);
    }


    public class UserConnectionStore
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _map = new();

        public void Add(string userId, string connectionId)
        {
            var set = _map.GetOrAdd(userId, _ => new HashSet<string>());
            lock (set) set.Add(connectionId);
        }

        public void Remove(string userId, string connectionId)
        {
            if (_map.TryGetValue(userId, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0) _map.TryRemove(userId, out _);
                }
            }
        }
        public IReadOnlyCollection<string> GetConnections(string userId) =>
            _map.TryGetValue(userId, out var set) ? set : Array.Empty<string>();
    }

}

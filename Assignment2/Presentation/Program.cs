//using BusinessLogic.Services;
using BusinessLogic.Services;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



//using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using static AppHub;
var builder = WebApplication.CreateBuilder(args);
// 1️⃣ Đăng ký DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2️⃣ Đăng ký Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequiredLength = 6;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = false;
//})
//.AddEntityFrameworkStores<FunewsManagementContext>()
//.AddDefaultTokenProviders();

builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllersWithViews(); builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian sống của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IArticleRepo,ArticleServices>();
builder.Services.AddScoped<ISystemAccountRepo, SystemAccountServices>();
builder.Services.AddScoped<ITagRepo, TagServices>(); 
builder.Services.AddScoped<ICategoryRepo, CategoryServices>();
builder.Services.AddSingleton<UserConnectionStore>(); // store cho AppHub
builder.Services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRazorPages();        // ✅ quan trọng
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Register/Login";       // route tới controller login của bạn
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
var app = builder.Build();
await SeedDefaultAdminAsync(app.Services, app.Configuration);
// ====== Hàm seed admin ======
async Task SeedDefaultAdminAsync(IServiceProvider services, IConfiguration config)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FunewsManagementContext>();

    await db.Database.MigrateAsync(); // Tự tạo database nếu chưa có

    var section = config.GetSection("DefaultAdminAccount");

    var email = section["AccountEmail"];
    var name = section["AccountName"];
    var password = section["AccountPassword"];
    var role = int.TryParse(section["AccountRole"], out var parsedRole) ? parsedRole : 1;

    // Kiểm tra xem admin đã tồn tại chưa
    var existingAdmin = await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountEmail == email);
    if (existingAdmin == null)
    {
        var admin = new SystemAccount
        {
            AccountName = name,
            AccountEmail = email,
            AccountPassword = password, // ⚠️ có thể hash sau
            AccountRole = role
        };

        db.SystemAccounts.Add(admin);
        await db.SaveChangesAsync();
        Console.WriteLine($"✅ Đã tạo tài khoản admin mặc định: {email}");
    }
    else
    {
        Console.WriteLine($"ℹ️ Tài khoản admin '{email}' đã tồn tại, bỏ qua seed.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // nếu có
app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();


app.MapHub<AppHub>("/hubs/app");
app.MapRazorPages();                    


app.Run();

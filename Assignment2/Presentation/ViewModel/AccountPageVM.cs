namespace Presentation.ViewModel
{
    public class AccountPageVM
    {
        public List<DataAccess.Models.SystemAccount> SystemAccounts { get; set; } = new();
        public AccountVM AccountVM { get; set; } = new();
    }
}

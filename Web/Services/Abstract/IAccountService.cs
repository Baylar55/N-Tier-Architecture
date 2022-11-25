using Web.ViewModels.Account;

namespace Web.Services.Abstract
{
    public interface IAccountService
    {
        Task<bool> RegisterUserAsync(AccountRegisterVM model);
        Task<bool> LoginUserAsync(AccountLoginVM model);
        Task LogoutAsync();
    }
}

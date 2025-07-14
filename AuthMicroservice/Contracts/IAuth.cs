using AuthMicroservice.Models;

namespace AuthMicroservice.Contracts
{
    public interface IAuth
    {
        Task<string> LoginAsync(LoginModel model);
        Task<bool> ValidateTokenAsync(string token);
        Task<string> RefreshTokenAsync(string token);
        Task LogoutAsync(string token);

        Task RegisterUser(User user);
    }
}

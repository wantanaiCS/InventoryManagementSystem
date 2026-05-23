using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string email, string password, int roleId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password);
    }
}

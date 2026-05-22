using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return null;

            // Verify password
            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        public async Task<User?> RegisterAsync(string username, string email, string password, int roleId)
        {
            // Check if username or email already exists
            if (await UsernameExistsAsync(username))
                return null;

            if (await EmailExistsAsync(email))
                return null;

            // Check if role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == roleId);
            if (!roleExists)
                return null;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                RoleId = roleId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Check if username exists
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verify password against hash
        /// Using simple hashing for now (BCrypt recommended for production)
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            // For production, use BCrypt.Net-Next:
            // return BCrypt.Net.BCrypt.Verify(password, hash);

            // Simple approach for development
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Hash password
        /// Using BCrypt for security
        /// </summary>
        public string HashPassword(string password)
        {
            // Use BCrypt for hashing
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

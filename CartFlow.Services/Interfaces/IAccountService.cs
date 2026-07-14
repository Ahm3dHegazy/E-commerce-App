using CartFlow.Data.Entities;

namespace CartFlow.Services.Interfaces;

public interface IAccountService
{
    // Auth methods
    Task<User?> SignInAsync(string email, string password);
    Task<User?> SignUpAsync(string firstName, string lastName, string email, string password);
    Task<User?> GetByIdAsync(int id);
    Task<User?> UpdateProfileAsync(int id, string firstName, string lastName, string email, string phone);

    Task<User> FindOrCreateExternalUserAsync(string email, string firstName, string lastName);

    // Password reset
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
}

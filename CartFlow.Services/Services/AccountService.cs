using CartFlow.Services.Interfaces;
using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public class AccountService(AppDbContext context) : IAccountService {
    public async Task<User?> SignInAsync(string email, string password) {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }
    public async Task<User?> SignUpAsync(string firstname, string lastName, string email, string password) {
        var user = new User {
            FirstName = firstname,
            LastName = lastName,
            Email = email,
            Password = password,
            UserRole = Role.CUSTOMER
        };

        context.Users.Add(user);
        try
        {
            await context.SaveChangesAsync();
            return user;
        }
        catch (DbUpdateException)
        {
            // Re-throw to be handled by the controller so we can return a user-friendly message
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(int id) {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> UpdateProfileAsync(int id, string firstName, string lastName, string email, string phone) {
        var user = await context.Users.FindAsync(id);
        if (user is null) return null;

        user.FirstName = firstName;
        user.LastName= lastName;
        user.Email = email;
        user.Phone = phone;

        await context.SaveChangesAsync();
        return user;
    }

}

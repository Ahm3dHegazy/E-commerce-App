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
        await context.SaveChangesAsync();
        return user;
    }


}

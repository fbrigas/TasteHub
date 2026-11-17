using TasteHub.Data;
using TasteHub.Entities;
using Microsoft.AspNetCore.Identity;

namespace TasteHub.Services;

public class UserService
{
    private readonly TasteHubDbContext _context;

    public UserService(TasteHubDbContext context)
    {
        _context = context;
    }

    public bool EmailExists(string email)
    {
        string normalizedEmail = email.Trim().ToUpperInvariant();

        return _context.Users.Any(user =>
            user.Email.ToUpper() == normalizedEmail);
    }

    public User RegisterUser(string name, string email, string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (EmailExists(normalizedEmail))
        {
            throw new ArgumentException("Este email já está registado.");
        }

        User user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            Role = "User",
            IsBlocked = false
        };

        PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }
}
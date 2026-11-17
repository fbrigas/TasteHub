using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TasteHub.Data;
using TasteHub.Entities;

namespace TasteHub.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using IServiceScope scope = services.CreateScope();

        TasteHubDbContext context =
            scope.ServiceProvider.GetRequiredService<TasteHubDbContext>();

        string? email = configuration["SeedAdmin:Email"];
        string? password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        email = email.Trim().ToLowerInvariant();

        bool adminExists = await context.Users
            .AnyAsync(user => user.Role == "Admin");

        if (adminExists)
        {
            return;
        }

        bool emailExists = await context.Users
            .AnyAsync(user => user.Email == email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "O email do administrador já pertence a outro utilizador."
            );
        }

        User admin = new User
        {
            Name = "Administrador TasteHub",
            Email = email,
            Role = "Admin",
            IsBlocked = false
        };

        PasswordHasher<User> hasher = new PasswordHasher<User>();

        admin.PasswordHash = hasher.HashPassword(admin, password);

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}

using FlowerShop.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Backend.Data;

public static class DataSeeder
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created and migrations applied
        await context.Database.MigrateAsync();

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Username = "admin"
            };
            
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");
            
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        // Seed some categories if empty
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Букеты", Description = "Готовые букеты" },
                new Category { Name = "Розы", Description = "Розы поштучно и в букетах" },
                new Category { Name = "Комнатные", Description = "Цветы в горшках" }
            );
            await context.SaveChangesAsync();
        }
    }
}

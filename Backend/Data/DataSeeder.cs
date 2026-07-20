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

        // Ожидаем применения миграций только для реляционных баз (не InMemory)
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }

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

        // Seed some flowers if empty
        if (!await context.Flowers.AnyAsync())
        {
            var category = await context.Categories.FirstOrDefaultAsync();
            if (category != null)
            {
                context.Flowers.AddRange(
                    new Flower { Name = "Композиция из гипсофилы 💙", Description = "Цветы и Воздух", Price = 1300, CategoryId = category.Id },
                    new Flower { Name = "Композиция", Description = "Цветы и Воздух", Price = 3800, CategoryId = category.Id },
                    new Flower { Name = "Композиция французских роз", Description = "Цветы и Воздух", Price = 3500, CategoryId = category.Id },
                    new Flower { Name = "Композиция пчелка 🐝", Description = "Цветы и Воздух", Price = 1900, CategoryId = category.Id },
                    new Flower { Name = "Вместо «Я тебя ЛЮБЛЮ 🌸»", Description = "Цветы и Воздух", Price = 3250, CategoryId = category.Id },
                    new Flower { Name = "Нежная зефирка", Description = "Цветы и Воздух", Price = 2950, CategoryId = category.Id },
                    new Flower { Name = "Голубое облачко", Description = "Цветы и Воздух", Price = 2850, CategoryId = category.Id },
                    new Flower { Name = "Персиковый букет", Description = "Цветы и Воздух", Price = 1800, CategoryId = category.Id },
                    new Flower { Name = "Букет мини бантики", Description = "Цветы и Воздух", Price = 1800, CategoryId = category.Id },
                    new Flower { Name = "Ароматные лилии", Description = "Цветы и Воздух", Price = 3650, CategoryId = category.Id },
                    new Flower { Name = "Нежнейшая зефирка", Description = "Цветы и Воздух", Price = 2450, CategoryId = category.Id },
                    new Flower { Name = "Букет кустовых роз", Description = "Цветы и Воздух", Price = 3500, CategoryId = category.Id },
                    new Flower { Name = "Ромашковый рай", Description = "Цветы и Воздух", Price = 3100, CategoryId = category.Id },
                    new Flower { Name = "Красное облачко", Description = "Цветы и Воздух", Price = 2850, CategoryId = category.Id },
                    new Flower { Name = "Букет БАНТ 🌸", Description = "Цветы и Воздух", Price = 3000, CategoryId = category.Id },
                    new Flower { Name = "Букет из хризантемы", Description = "Цветы и Воздух", Price = 1550, CategoryId = category.Id },
                    new Flower { Name = "Букет Нежный", Description = "Цветы и Воздух", Price = 2550, CategoryId = category.Id },
                    new Flower { Name = "Шары", Description = "Шары (бесплатно / уточняйте)", Price = 0, CategoryId = category.Id }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}

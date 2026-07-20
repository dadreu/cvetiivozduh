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
                    new Flower { Name = "Композиция из гипсофилы 💙", Description = "Объемное воздушное облако из стойкой гипсофилы", Price = 1300, ImageUrl = "/catalog/gypsofils.jpg", CategoryId = category.Id },
                    new Flower { Name = "Композиция", Description = "Стильная сборная композиция в шляпной коробке", Price = 3800, ImageUrl = "/catalog/compose2.jpg", CategoryId = category.Id },
                    new Flower { Name = "Композиция французских роз", Description = "Элегантные вывернутые французские розы", Price = 3500, ImageUrl = "/catalog/frenchRose.jpg", CategoryId = category.Id },
                    new Flower { Name = "Композиция пчелка 🐝", Description = "Яркая летняя композиция, поднимающая настроение", Price = 1900, ImageUrl = "/catalog/bee.jpg", CategoryId = category.Id },
                    new Flower { Name = "Вместо «Я тебя ЛЮБЛЮ 🌸»", Description = "Романтичный букет для самых важных признаний", Price = 3250, ImageUrl = "/catalog/i love u.jpg", CategoryId = category.Id },
                    new Flower { Name = "Нежная зефирка", Description = "Мягкие пудровые оттенки в воздушной упаковке", Price = 2950, ImageUrl = "/catalog/softy.jpg", CategoryId = category.Id },
                    new Flower { Name = "Голубое облачко", Description = "Небесно-голубой букет для утонченных натур", Price = 2850, CategoryId = category.Id },
                    new Flower { Name = "Персиковый букет", Description = "Теплые и сочные персиковые тона", Price = 1800, CategoryId = category.Id },
                    new Flower { Name = "Букет мини бантики", Description = "Очаровательный мини-букет с бантиками", Price = 1800, CategoryId = category.Id },
                    new Flower { Name = "Ароматные лилии", Description = "Роскошные белые лилии с глубоким ароматом", Price = 3650, ImageUrl = "/catalog/Lylyi.jpg", CategoryId = category.Id },
                    new Flower { Name = "Нежнейшая зефирка", Description = "Максимальная легкость и нежность в каждом лепестке", Price = 2450, CategoryId = category.Id },
                    new Flower { Name = "Букет кустовых роз", Description = "Классика, которая никогда не выйдет из моды", Price = 3500, CategoryId = category.Id },
                    new Flower { Name = "Ромашковый рай", Description = "Полевая свежесть и простота ромашек", Price = 3100, CategoryId = category.Id },
                    new Flower { Name = "Красное облачко", Description = "Страстный и яркий букет красных оттенков", Price = 2850, CategoryId = category.Id },
                    new Flower { Name = "Букет БАНТ 🌸", Description = "Стильный букет с акцентным декоративным бантом", Price = 3000, ImageUrl = "/catalog/bant.jpg", CategoryId = category.Id },
                    new Flower { Name = "Букет из хризантемы", Description = "Стойкие и пышные кустовые хризантемы", Price = 1550, CategoryId = category.Id },
                    new Flower { Name = "Букет Нежный", Description = "Легкий цветочный микс в пастельных тонах", Price = 2550, CategoryId = category.Id },
                    new Flower { Name = "Шары", Description = "Шары (бесплатно / уточняйте)", Price = 0, CategoryId = category.Id }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}

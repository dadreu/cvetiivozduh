using FlowerShop.Backend.Data;
using FlowerShop.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Backend.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin").RequireAuthorization();

        group.MapPost("/flowers", async (Flower flower, AppDbContext dbContext) =>
        {
            dbContext.Flowers.Add(flower);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/catalog/flowers/{flower.Id}", flower);
        });

        group.MapPut("/flowers/{id}", async (int id, Flower updatedFlower, AppDbContext dbContext) =>
        {
            var flower = await dbContext.Flowers.FindAsync(id);
            if (flower == null) return Results.NotFound();

            flower.Name = updatedFlower.Name;
            flower.Description = updatedFlower.Description;
            flower.Price = updatedFlower.Price;
            flower.CategoryId = updatedFlower.CategoryId;
            flower.IsWide = updatedFlower.IsWide;
            if (!string.IsNullOrEmpty(updatedFlower.ImageUrl))
            {
                flower.ImageUrl = updatedFlower.ImageUrl;
            }

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/flowers/{id}", async (int id, AppDbContext dbContext) =>
        {
            var flower = await dbContext.Flowers.FindAsync(id);
            if (flower == null) return Results.NotFound();

            dbContext.Flowers.Remove(flower);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPost("/upload", async (IFormFile file, IWebHostEnvironment env) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is empty.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(ext))
                return Results.BadRequest("Invalid file extension.");

            var newFileName = $"{Guid.NewGuid()}{ext}";
            
            // In a real scenario, this might be /var/www/flowershop/uploads outside the app folder
            // For local development, we put it in wwwroot/uploads or similar.
            var uploadPath = Path.Combine(env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, newFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{newFileName}";
            return Results.Ok(new { imageUrl });
        });
        // Vacancies CRUD
        group.MapGet("/vacancies", async (AppDbContext dbContext) =>
        {
            return Results.Ok(await dbContext.Vacancies.AsNoTracking().ToListAsync());
        });

        group.MapPost("/vacancies", async (Vacancy vacancy, AppDbContext dbContext) =>
        {
            dbContext.Vacancies.Add(vacancy);
            await dbContext.SaveChangesAsync();
            return Results.Created($"/api/admin/vacancies", vacancy);
        });

        group.MapPut("/vacancies/{id}", async (int id, Vacancy updated, AppDbContext dbContext) =>
        {
            var vacancy = await dbContext.Vacancies.FindAsync(id);
            if (vacancy == null) return Results.NotFound();

            vacancy.Title = updated.Title;
            vacancy.Description = updated.Description;
            vacancy.IsActive = updated.IsActive;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/vacancies/{id}", async (int id, AppDbContext dbContext) =>
        {
            var vacancy = await dbContext.Vacancies.FindAsync(id);
            if (vacancy == null) return Results.NotFound();

            dbContext.Vacancies.Remove(vacancy);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}

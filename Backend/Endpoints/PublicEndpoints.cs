using FlowerShop.Backend.Data;
using FlowerShop.Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Backend.Endpoints;

public static class PublicEndpoints
{
    public static void MapPublicEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/catalog");

        group.MapGet("/flowers", async (AppDbContext dbContext) =>
        {
            var flowers = await dbContext.Flowers
                .AsNoTracking()
                .Select(f => new FlowerDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    ImageUrl = f.ImageUrl,
                    IsWide = f.IsWide
                })
                .ToListAsync();

            return Results.Ok(flowers);
        });

        group.MapGet("/flowers/{id}", async (int id, AppDbContext dbContext) =>
        {
            var flower = await dbContext.Flowers
                .AsNoTracking()
                .Include(f => f.Category)
                .Where(f => f.Id == id)
                .Select(f => new FlowerDetailDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    ImageUrl = f.ImageUrl,
                    CategoryId = f.CategoryId,
                    CategoryName = f.Category.Name,
                    IsWide = f.IsWide
                })
                .FirstOrDefaultAsync();

            return flower != null ? Results.Ok(flower) : Results.NotFound();
        });
        
        group.MapGet("/categories", async (AppDbContext dbContext) =>
        {
            var categories = await dbContext.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Results.Ok(categories);
        });

        group.MapGet("/vacancies", async (AppDbContext dbContext) =>
        {
            var vacancies = await dbContext.Vacancies
                .AsNoTracking()
                .Where(v => v.IsActive)
                .ToListAsync();

            return Results.Ok(vacancies);
        });
    }
}

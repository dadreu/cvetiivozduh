using Microsoft.EntityFrameworkCore;
using FlowerShop.Backend.Models;

namespace FlowerShop.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet will be added here once we create the models
    public DbSet<Flower> Flowers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
}

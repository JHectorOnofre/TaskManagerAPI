using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }
    public DbSet<TaskItem> Tasks { get; set; }

    public DbSet<Category> Categories { get; set; } // Modelo agregado 8enero
}
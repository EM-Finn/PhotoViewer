using Microsoft.EntityFrameworkCore;

using PhotoViewer.Core.Models;

namespace PhotoViewer.Database.Context;

public class AppDbContext : DbContext
{
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            "Data Source=photo_viewer.db");
    }
}
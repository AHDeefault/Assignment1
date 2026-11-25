using Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EfcRepositories;

public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        string databasePath = Path.GetFullPath(
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "EfcRepositories",
                "app.db"));

        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }
}
using DevLife.Infrastructure.Database.Postgres.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Infrastructure.Database.Postgres;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}
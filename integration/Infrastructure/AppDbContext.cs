using integration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace integration.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<DataEntity> DataRecords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataEntity>(e => 
        {
            e.Property(x => x.CreatedAt).IsRequired();
            e.HasIndex(x => x.CreatedAt); // Для эффективной очистки
        });
    }
}
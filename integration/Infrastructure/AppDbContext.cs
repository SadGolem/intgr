using integration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace integration.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<LocationEntity> LocationRecords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(e => e.IdAsuPro).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.HasIndex(e => e.ExpirationDate)
                .HasDatabaseName("IX_LocationEntities_ExpirationDate");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(9,6)");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(9,6)");
        });
    }
}
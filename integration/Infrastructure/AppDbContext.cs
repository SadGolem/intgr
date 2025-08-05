using integration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace integration.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<LocationEntity> LocationRecords { get; set; }
    public DbSet<LocationEntity> ClientsRecords { get; set; }
    public DbSet<LocationEntity> EmitterRecords { get; set; }
    public DbSet<LocationEntity> ScheduleRecords { get; set; }
    public DbSet<LocationEntity> EntryRecords { get; set; }

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
        
        modelBuilder.Entity<ClientEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdAsuPro).IsRequired();
            entity.Property(e => e.Doc_type).IsRequired();
            entity.Property(e => e.Type_ka).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.HasIndex(e => e.ExpirationDate)
                .HasDatabaseName("IX_ClientEntities_ExpirationDate");
        });
        
        modelBuilder.Entity<EmitterEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WasteSource_Id).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.HasIndex(e => e.ExpirationDate)
                .HasDatabaseName("IX_EmitterEntities_ExpirationDate");
        });
        
        modelBuilder.Entity<ScheduleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdAsuPro);
            entity.Property(e => e.Schedule).IsRequired();
            entity.Property(e => e.IdLocation).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.HasIndex(e => e.ExpirationDate)
                .HasDatabaseName("IX_ScheduleEntities_ExpirationDate");
        });
        
        modelBuilder.Entity<EntryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IsAsuPro).IsRequired();
            entity.Property(e => e.IdLocation).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpirationDate).IsRequired();
            entity.HasIndex(e => e.ExpirationDate)
                .HasDatabaseName("IX_EntryEntities_ExpirationDate");
        });
    }
}
// integration/Context/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using integration.Domain.Entities;
using integration.Domain.Entities.Staging;

public class AppDbContext : DbContext
{
    public DbSet<AproSnapshot> AproSnapshots => Set<AproSnapshot>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractPosition> ContractPositions => Set<ContractPosition>();
    public DbSet<Assignee> Assignees => Set<Assignee>();
    public DbSet<Boss> Bosses => Set<Boss>();
    public DbSet<RootCompany> RootCompanies => Set<RootCompany>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Emitter> Emitters => Set<Emitter>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Container> Containers => Set<Container>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<WasteSource> WasteSources => Set<WasteSource>();
    public DbSet<WasteSourceCategory> WasteSourceCategories => Set<WasteSourceCategory>();
    public DbSet<Entry> Entries => Set<Entry>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Схемы
        b.HasDefaultSchema("core");
        b.Entity<AproSnapshot>().ToTable("apro_snapshots", "stg");

        // AproSnapshot (TTL + индексы)
        b.Entity<AproSnapshot>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
            e.Property(x => x.ReceivedAt).HasDefaultValueSql("timezone('utc', now())");
            e.Property(x => x.ExpiresAt).HasDefaultValueSql("timezone('utc', now()) + interval '10 days'");
            e.HasIndex(x => x.ExpiresAt);
            e.HasIndex(x => new { x.Entity, x.ExternalId });
            e.HasIndex(x => x.Hash);
        });

        // ===== Доменные связи (исправляем ошибки) =====

        // Уникальность внешних ID там, где он есть
        b.Entity<Client>().HasIndex(x => x.IdAsuPro).IsUnique();
        b.Entity<Contract>().HasIndex(x => x.ExternalId).IsUnique();
        b.Entity<ContractPosition>().HasIndex(x => x.ExternalId).IsUnique();
        b.Entity<Location>().HasIndex(x => x.ExternalId).IsUnique();
        b.Entity<Status>().HasIndex(x => new { x.ExternalId, x.EntityType }).IsUnique();
        b.Entity<WasteSource>().HasIndex(x => x.ExternalId).IsUnique();

        // Client -> RootCompany, Boss (Restrict, чтобы не ловить циклы)
        b.Entity<Client>()
            .HasOne(c => c.RootCompany)
            .WithMany(rc => rc.Clients)
            .HasForeignKey(c => c.RootCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Client>()
            .HasOne(c => c.Boss)
            .WithMany(boss => boss.Clients)
            .HasForeignKey(c => c.BossId)
            .OnDelete(DeleteBehavior.Restrict);

        // Location -> Status (required)
        b.Entity<Location>()
            .HasOne(l => l.Status)
            .WithMany()
            .HasForeignKey(l => l.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // ДВА разных FK на Client в Location: ParticipantId и ClientId.
        // Нужны РАЗНЫЕ обратные коллекции, иначе EF ругается на неоднозначность.
        // см. изменения в сущности Client ниже.
        b.Entity<Location>()
            .HasOne(l => l.Participant)
            .WithMany(c => c.ParticipantLocations)
            .HasForeignKey(l => l.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Location>()
            .HasOne(l => l.Client)
            .WithMany(c => c.CustomerLocations)
            .HasForeignKey(l => l.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Типы координат для PG (избежать float)
        b.Entity<Location>().Property(l => l.Lat).HasColumnType("numeric(9,6)");
        b.Entity<Location>().Property(l => l.Lon).HasColumnType("numeric(9,6)");

        // Contract
        b.Entity<Contract>()
            .HasOne(c => c.Status)
            .WithMany()
            .HasForeignKey(c => c.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany(c => c.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade); // удаляешь клиента — уедут его контракты

        b.Entity<Contract>()
            .HasOne(c => c.Assignee)
            .WithMany(a => a.Contracts)
            .HasForeignKey(c => c.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ContractPosition
        b.Entity<ContractPosition>()
            .HasOne(cp => cp.Status)
            .WithMany()
            .HasForeignKey(cp => cp.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<ContractPosition>()
            .HasOne(cp => cp.Contract)
            .WithMany(c => c.Positions)
            .HasForeignKey(cp => cp.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ContractPosition>()
            .HasOne(cp => cp.Emitter)
            .WithMany(e => e.ContractPositions)
            .HasForeignKey(cp => cp.EmitterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<ContractPosition>()
            .HasOne(cp => cp.Location)
            .WithMany(l => l.ContractPositions)
            .HasForeignKey(cp => cp.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Emitter -> WasteSource
        b.Entity<Emitter>()
            .HasOne(e => e.WasteSource)
            .WithMany(ws => ws.Emitters)
            .HasForeignKey(e => e.WasteSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        // WasteSource -> Category
        b.Entity<WasteSource>()
            .HasOne(ws => ws.Category)
            .WithMany(c => c.WasteSources)
            .HasForeignKey(ws => ws.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule, Container, Entry
        b.Entity<Schedule>()
            .HasOne(s => s.Location)
            .WithMany(l => l.Schedules)
            .HasForeignKey(s => s.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Schedule>()
            .HasOne(s => s.Emitter)
            .WithMany(e => e.Schedules)
            .HasForeignKey(s => s.EmitterId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Container>()
            .HasOne(cn => cn.Schedule)
            .WithMany(s => s.Containers)
            .HasForeignKey(cn => cn.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Entry>()
            .HasOne(e => e.Location)
            .WithMany(l => l.Entries)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

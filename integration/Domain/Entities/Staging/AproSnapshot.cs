// integration/Domain/Entities/Staging/AproSnapshot.cs
using System.Text.Json;

namespace integration.Domain.Entities.Staging;

public class AproSnapshot
{
    public long Id { get; set; }                  // bigserial
    public string Source { get; set; } = "APRO";  // источник
    public string Entity { get; set; } = null!;   // "client", "contract", "schedule", ...
    public int ExternalId { get; set; }           // внешний id из АПРО
    public string Payload { get; set; } = null!;  // JSONB
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(10);
    public bool Processed { get; set; }           // помечаем после успешной проекции в core
    public DateTime? ProcessedAt { get; set; }
    public string? Hash { get; set; }             // для дедупликации (SHA256 от Payload)
}
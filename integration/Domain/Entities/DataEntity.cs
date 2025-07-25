namespace integration.Domain.Entities;

public class DataEntity : EntityBase
{
    public string Payload { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private DataEntity() { }

    public static DataEntity Create(string payload) => new() { Payload = payload };
}


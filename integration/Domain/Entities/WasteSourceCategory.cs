namespace integration.Domain.Entities;

public class WasteSourceCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public List<WasteSource> WasteSources { get; set; } = new();
}
namespace Api.Dtos.Audit;

public class AuditDto<TKey>
{
    public int Id { get; set; }

    public required TKey EntityId { get; set; }

    public string? OldObject { get; set; }

    public string? NewObject { get; set; }

    public required string ChangedBy { get; set; }

    public required DateTime ChangedAt { get; set; }

    public required string ChangeType { get; set; }
}

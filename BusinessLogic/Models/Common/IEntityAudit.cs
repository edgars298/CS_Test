namespace BusinessLogic.Models.Common;

public interface IEntityAudit<TKey>
{
    public int Id { get; set; }

    public TKey EntityId { get; set; }

    public string? OldObject { get; set; }

    public string? NewObject { get; set; }

    public string ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; }

    public string ChangeType { get; set; }
}

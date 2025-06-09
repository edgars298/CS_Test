using BusinessLogic.Models.Common;

namespace BusinessLogic.Models;

public class ProductAudit : IEntityAudit<int>
{
    public int Id { get; set; }

    public int EntityId { get; set; }

    public string? OldObject { get; set; }

    public string? NewObject { get; set; }

    public string ChangedBy { get; set; } = default!;

    public DateTime ChangedAt { get; set; }

    public string ChangeType { get; set; } = default!;
}

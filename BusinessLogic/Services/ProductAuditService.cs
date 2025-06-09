using BusinessLogic.Core;
using BusinessLogic.Models;

namespace BusinessLogic.Services;

public class ProductAuditService : BaseService<ProductAudit>, IProductAuditService
{
    public ProductAuditService(IApplicationDbContext context)
        : base(context)
    {
    }

    public IQueryable<ProductAudit> GetByDateRange(DateOnly from, DateOnly to)
    {
        return dbset
            .Where(pa => pa.ChangedAt >= from.ToDateTime(TimeOnly.MinValue) && pa.ChangedAt <= to.ToDateTime(TimeOnly.MaxValue));
    }
}

public interface IProductAuditService : IBaseService<ProductAudit>
{
    IQueryable<ProductAudit> GetByDateRange(DateOnly from, DateOnly to);
}

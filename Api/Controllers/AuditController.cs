using Api.Dtos.Audit;
using BusinessLogic.Core;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("audit")]
[Authorize(Roles = $"{RoleNames.Admin}")]
public class AuditController(IProductAuditService _productAuditService) : ControllerBase
{
    private readonly IProductAuditService _productAuditService = _productAuditService;

    [HttpGet]
    public async Task<IActionResult> GetAsync(DateOnly? from, DateOnly? to)
    {
        var productAudits = _productAuditService.GetByDateRange(from ?? DateOnly.MinValue, to ?? DateOnly.MaxValue);

        var mappedProductAudits = await productAudits.Select(pa => new AuditDto<int>
        {
            Id = pa.Id,
            EntityId = pa.EntityId,
            OldObject = pa.OldObject,
            NewObject = pa.NewObject,
            ChangedBy = pa.ChangedBy,
            ChangedAt = pa.ChangedAt,
            ChangeType = pa.ChangeType
        }).ToListAsync();

        return Ok(mappedProductAudits);
    }
}

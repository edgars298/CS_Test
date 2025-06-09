using BusinessLogic.Models;
using BusinessLogic.Models.Common;
using BusinessLogic.Models.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace BusinessLogic.Core;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }

    public DbSet<ProductAudit> ProductAudits { get; set; }

    private readonly string _currentUser;

    private readonly List<(object Entity, object Audit)> _pendingAudits = [];

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _currentUser = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ProductConfig());
    }

    public async Task<int> SaveChangesAsync()
    {
        CreateAuditEntries<Product, ProductAudit, int>();

        var result = await base.SaveChangesAsync();

        FinalizePendingAudits<Product, ProductAudit, int>(p => p.Id);

        if (_pendingAudits.Any())
        {
            _pendingAudits.Clear();
            await base.SaveChangesAsync();
        }

        return result;
    }

    #region PRIVATE
    private void FinalizePendingAudits<TEntity, TAudit, TKey>(Func<TEntity, TKey> keySelector)
        where TEntity : class
        where TAudit : class, IEntityAudit<TKey>
    {
        foreach (var (entityObj, auditObj) in _pendingAudits)
        {
            if (entityObj is TEntity entity && auditObj is TAudit audit)
            {
                audit.EntityId = keySelector(entity);
                audit.NewObject = JsonSerializer.Serialize(entity);
                Set<TAudit>().Add(audit);
            }
        }
    }


    private void CreateAuditEntries<TEntity, TAudit, TKey>()
        where TEntity : class
        where TAudit : class, IEntityAudit<TKey>, new()
    {
        var entries = ChangeTracker.Entries<TEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var keyProp = entry.Metadata.FindPrimaryKey()?.Properties.ToList().FirstOrDefault();

            if (keyProp is null)
            {
                continue;
            }

            var keyValue = entry.State is EntityState.Added
                ? default
                : (TKey?)entry.Property(keyProp.Name).CurrentValue;

            if (keyValue is null)
            {
                continue;
            }

            var before = entry.State is EntityState.Added ? null : entry.OriginalValues.ToObject();
            var after = entry.State is EntityState.Deleted ? null : entry.CurrentValues.ToObject();

            var audit = new TAudit
            {
                EntityId = keyValue,
                OldObject = before is null ? null : JsonSerializer.Serialize(before),
                NewObject = after is null ? null : JsonSerializer.Serialize(after),
                ChangedBy = _currentUser,
                ChangedAt = now,
                ChangeType = entry.State.ToString()
            };

            if (entry.State is EntityState.Added)
            {
                _pendingAudits.Add((entry.Entity, audit));
            }
            else
            {
                Set<TAudit>().Add(audit);
            }
        }
    }
    #endregion
}

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }

    DbSet<ProductAudit> ProductAudits { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync();
}
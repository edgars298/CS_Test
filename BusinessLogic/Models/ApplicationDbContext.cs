using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Models;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
}

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    int SaveChanges();
}
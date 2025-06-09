using BusinessLogic.Core;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class BaseService<T>(IApplicationDbContext context) where T : class
{
    protected readonly IApplicationDbContext context = context;
    protected readonly DbSet<T> dbset = context.Set<T>();

    public async virtual Task AddAsync(T entity)
    {
        await dbset.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async virtual Task UpdateAsync(T entity)
    {
        dbset.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async virtual Task DeleteAsync(T entity)
    {
        dbset.Remove(entity);

        await context.SaveChangesAsync();
    }

    public async virtual Task<T?> FindAsync(int id)
    {
        return await dbset.FindAsync(id);
    }

    public virtual IQueryable<T> GetAll()
    {
        return dbset;
    }
}

public interface IBaseService<T> where T : class
{
    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<T?> FindAsync(int id);

    IQueryable<T> GetAll();
}

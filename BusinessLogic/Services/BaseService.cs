using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class BaseService<T>(IApplicationDbContext context) where T : class
{
    protected readonly IApplicationDbContext context = context;
    protected readonly DbSet<T> dbset = context.Set<T>();

    public virtual void Add(T entity)
    {
        dbset.Add(entity);
        context.SaveChanges();
    }

    public virtual T? Find(int id)
    {
        return dbset.Find(id);
    }
}

public interface IBaseService<T> where T : class
{
    void Add(T entity);

    T? Find(int id);
}

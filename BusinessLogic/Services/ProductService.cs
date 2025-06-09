using BusinessLogic.Core;
using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class ProductService : BaseService<Product>, IProductService
{
    public ProductService(IApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> FindExistingAsync(Product product)
    {
        return await dbset
            .FirstOrDefaultAsync(p => p.Price == product.Price && p.Quantity == product.Quantity && p.Title == product.Title);
    }
}

public interface IProductService : IBaseService<Product>
{
    Task<Product?> FindExistingAsync(Product product);
}

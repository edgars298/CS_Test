using Api.Core.Helpers;
using Api.Dtos.Product;
using BusinessLogic.Core;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("products")]
[Authorize]
public class ProductController(IProductService _productService, ICalculationHelper _calculationHelper) : ControllerBase
{
    private readonly IProductService _productService = _productService;
    private readonly ICalculationHelper _calculationHelper = _calculationHelper;

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var products = await _productService.GetAll().Select(p => new ProductListItemDto
        {
            Id = p.Id,
            Name = p.Title,
            Quantity = p.Quantity,
            Price = p.Price,
            TotalPriceWithVat = _calculationHelper.CalculatePriceWithVat(p.Quantity, p.Price)
        }).ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindByIdAsync(int id)
    {
        var product = await _productService.FindAsync(id);

        if (product is null)
        {
            return NotFound("Product does not exist.");
        }

        return Ok(new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Quantity = product.Quantity,
            Price = product.Price,
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Admin}")]
    public async Task<IActionResult> CreateAsync(ProductFormDto form)
    {
        var product = new Product
        {
            Title = form.Title,
            Quantity = form.Quantity,
            Price = form.Price
        };

        await _productService.AddAsync(product);

        return NoContent();
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{RoleNames.Admin}")]
    public async Task<IActionResult> UpdateAsync(ProductFormDto form, int id)
    {
        var product = await _productService.FindAsync(id);

        if (product is null)
        {
            return NotFound("Product does not exist.");
        }

        product.Title = form.Title;
        product.Quantity = form.Quantity;
        product.Price = form.Price;

        await _productService.UpdateAsync(product);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{RoleNames.Admin}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var product = await _productService.FindAsync(id);

        if (product is null)
        {
            return NotFound("Product does not exist.");
        }

        await _productService.DeleteAsync(product);

        return NoContent();
    }
}

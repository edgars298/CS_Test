using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BusinessLogic.Core;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        await CreateProductAsync(new()
        {
            Title = "Test Product 1",
            Quantity = 100,
            Price = 10.00M
        }, productService);

        await CreateProductAsync(new()
        {
            Title = "Test Product 2",
            Quantity = 200,
            Price = 5.00M
        }, productService);

        await CreateProductAsync(new()
        {
            Title = "Test Product 3",
            Quantity = 300,
            Price = 1.00M
        }, productService);

        await CreateRolesAsync(roleManager);

        await CreateUserAsync("admin@example.com", "Qwerty12345!", RoleNames.Admin, userManager);
    }

    #region PRIVATE
    private async static Task CreateProductAsync(Product model, IProductService productService)
    {
        var product = await productService.FindExistingAsync(model);

        if (product is not null)
        {
            return;
        }

        await productService.AddAsync(model);
    }

    private async static Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = typeof(RoleNames)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral || f.IsInitOnly)
            .Select(f => f.GetValue(null)?.ToString())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        foreach (var role in roles)
        {
            if (role is not null && !await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async static Task CreateUserAsync(string userName, string password, string role, UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(userName);

        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
    #endregion
}

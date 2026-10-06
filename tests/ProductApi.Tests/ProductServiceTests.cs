using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Repositories;
using ProductApi.Services;

namespace ProductApi.Tests;

public class ProductServiceTests
{
    // Each test gets its own isolated in-memory database.
    private static ProductService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProductService(new ProductRepository(new AppDbContext(options)));
    }

    private static ProductRequest Valid(string name = "Keyboard") => new()
    {
        Name = name, Description = "Mechanical", Price = 49.99m, StockQuantity = 10, IsActive = true
    };

    [Fact]
    public async Task Create_SavesProduct_AndSetsIdAndCreatedAt()
    {
        var svc = CreateService();

        var created = await svc.CreateAsync(Valid());

        Assert.True(created.Id > 0);
        Assert.Equal("Keyboard", created.Name);
        Assert.Equal(49.99m, created.Price);
        Assert.NotEqual(default, created.CreatedAt);
        Assert.Null(created.UpdatedAt);
    }

    [Fact]
    public async Task Create_TrimsName()
    {
        var created = await CreateService().CreateAsync(Valid("  Mouse  "));
        Assert.Equal("Mouse", created.Name);
    }

    [Fact]
    public async Task GetById_ReturnsProduct_WhenExists()
    {
        var svc = CreateService();
        var created = await svc.CreateAsync(Valid());

        var found = await svc.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenMissing()
    {
        Assert.Null(await CreateService().GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        var svc = CreateService();
        await svc.CreateAsync(Valid("A"));
        await svc.CreateAsync(Valid("B"));

        var all = await svc.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("   ", 1, 1)]
    [InlineData("Ok", -1, 1)]
    [InlineData("Ok", 1, -5)]
    public async Task Create_Throws_OnInvalidInput(string name, decimal price, int stock)
    {
        var request = new ProductRequest { Name = name, Price = price, StockQuantity = stock };
        await Assert.ThrowsAsync<ArgumentException>(() => CreateService().CreateAsync(request));
    }

    [Fact]
    public void RequestDto_DataAnnotations_RejectNegativePriceAndEmptyName()
    {
        var request = new ProductRequest { Name = "", Price = -1 };
        var results = new List<ValidationResult>();

        var ok = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.False(ok);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ProductRequest.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ProductRequest.Price)));
    }

    [Fact]
    public async Task Update_ChangesFields_AndSetsUpdatedAt()
    {
        var svc = CreateService();
        var created = await svc.CreateAsync(Valid());

        var updated = await svc.UpdateAsync(created.Id, new ProductRequest
        {
            Name = "Keyboard Pro", Price = 99m, StockQuantity = 3, IsActive = false
        });

        Assert.NotNull(updated);
        Assert.Equal("Keyboard Pro", updated!.Name);
        Assert.Equal(99m, updated.Price);
        Assert.False(updated.IsActive);
        Assert.NotNull(updated.UpdatedAt);
        Assert.Equal(created.CreatedAt, updated.CreatedAt);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenMissing()
    {
        Assert.Null(await CreateService().UpdateAsync(42, Valid()));
    }

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        var svc = CreateService();
        var created = await svc.CreateAsync(Valid());

        Assert.True(await svc.DeleteAsync(created.Id));
        Assert.Null(await svc.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenMissing()
    {
        Assert.False(await CreateService().DeleteAsync(42));
    }
}

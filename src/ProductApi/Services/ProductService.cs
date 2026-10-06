using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Services;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task<ProductResponse?> UpdateAsync(int id, ProductRequest request);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<List<ProductResponse>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToResponse).ToList();

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        Validate(request);
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        return ToResponse(await _repo.AddAsync(product));
    }

    public async Task<ProductResponse?> UpdateAsync(int id, ProductRequest request)
    {
        Validate(request);
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return null;

        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(product);
        return ToResponse(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return false;
        await _repo.DeleteAsync(product);
        return true;
    }

    // Business rules, enforced even if the service is called outside the HTTP pipeline.
    private static void Validate(ProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");
        if (request.Price < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (request.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");
    }

    private static ProductResponse ToResponse(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive, p.CreatedAt, p.UpdatedAt);
}

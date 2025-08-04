using desafio_t2m.Domain;
using desafio_t2m.Dto;
using desafio_t2m.Utils;

namespace desafio_t2m.Service;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetProductByNameAsync(string name)
    {
        return await _repository.GetByNameAsync(name);
    }

    public async Task AddProductAsync(ProductDTO productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.Name))
            throw new ArgumentException("O Nome do Produto não pode estar vazio.");

        var product = new Product(
            productDto.Name,
            productDto.Quantity,
            productDto.Description,
            productDto.Price
        );

        await _repository.AddAsync(product);
    }

    public async Task UpdateProductAsync(string name, ProductDTO productDto)
    {
        var normalizedName = NameNormalizer.Normalize(name);
        var existing = await _repository.GetByNameAsync(normalizedName);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        existing.Name = productDto.Name;
        existing.Quantity = productDto.Quantity;
        existing.Price = productDto.Price;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteProductAsync(string name)
    {
        var normalizedName = NameNormalizer.Normalize(name);
        var existing = await _repository.GetByNameAsync(normalizedName);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        await _repository.DeleteAsync(existing.Id);
    }
}
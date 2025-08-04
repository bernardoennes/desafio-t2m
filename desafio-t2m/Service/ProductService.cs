using desafio_t2m.Domain;

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

	public async Task<Product?> GetProductByIdAsync(long id)
	{
		return await _repository.GetByIdAsync(id);
	}

	public async Task AddProductAsync(Product product)
	{
		if (string.IsNullOrWhiteSpace(product.Name))
			throw new ArgumentException("O Nome do Produto não pode estar vazio.");

		await _repository.AddAsync(product);
	}

	public async Task UpdateProductAsync(Product product)
	{
		var existing = await _repository.GetByIdAsync(product.Id);
		if (existing is null)
			throw new InvalidOperationException("O Produto informado não foi encontrado.");

		await _repository.UpdateAsync(product);
	}

	public async Task DeleteProductAsync(long id)
	{
		var existing = await _repository.GetByIdAsync(id);
		if (existing is null)
			throw new InvalidOperationException("O Produto informado não foi encontrado.");

		await _repository.DeleteAsync(id);
	}
}
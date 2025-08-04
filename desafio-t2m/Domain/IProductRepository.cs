using desafio_t2m.Domain;

public interface IProductRepository
{
	Task<IEnumerable<Product>> GetAllAsync();
	Task<Product?> GetByIdAsync(long id);
	Task<long> AddAsync(Product product);
	Task<bool> UpdateAsync(Product product);
	Task<bool> DeleteAsync(long id);
}
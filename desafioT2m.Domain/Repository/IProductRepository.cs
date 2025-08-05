using desafioT2m.Domain;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAll();
    Task<Product?> GetById(long id);
    Task<long> Add(Product product);
    Task<bool> Update(Product product);
    Task<bool> Delete(long id);
}
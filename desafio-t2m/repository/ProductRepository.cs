using System.Data;
using Dapper;
using Npgsql;
using desafio_t2m.Domain;

/* Glossario
IEnumerable: Modelo generico que representa uma cole��o de objetos do tipo especificado.
Task: Variavel que representa uma opera��o ass�ncrona que pode ser aguardada.
readonly: Indica que a propriedade � somente leitura, ou seja, n�o pode ser modificada ap�s a inicializa��o.
*/

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
    }

    private IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM products";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM products WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<long> AddAsync(Product product)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO products (name, quantity, price)
            VALUES (@Name, @Quantity, @Price)
            RETURNING id;
        ";
        var id = await connection.ExecuteScalarAsync<long>(sql, product);
        product.Id = id;
        return id;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE products
            SET name = @Name,
                quantity = @Quantity,
                price = @Price
            WHERE id = @Id;
        ";
        var affectedRows = await connection.ExecuteAsync(sql, product);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM products WHERE id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
}

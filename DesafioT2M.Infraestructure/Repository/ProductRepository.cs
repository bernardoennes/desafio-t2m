using Dapper;
using desafioT2m.Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

/* Glossario
IEnumerable: Modelo generico que representa uma coleção de objetos do tipo especificado.
Task: Variavel que representa uma operação assíncrona que pode ser aguardada.
readonly: Indica que a propriedade é somente leitura, ou seja, não pode ser modificada após a inicialização.
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

    public async Task<IEnumerable<Product>> GetAll()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM products";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetById(long id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM products WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<Product?> GetByBarCode(string barCode)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM products WHERE barcode = @BarCode";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { BarCode = barCode });
    }

    public async Task<long> Add(Product product)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO products (barcode, name, quantity, description, price)
            VALUES (@barcode, @name, @quantity, @description, @price)
            RETURNING id;
        ";
        var id = await connection.ExecuteScalarAsync<long>(sql, product);
        product.Id = id;
        return id;
    }

    public async Task<bool> Update(Product product)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE products
            SET barcode = @barcode,
                name = @name,
                quantity = @quantity,
                description = @description,
                price = @price
            WHERE id = @id;
        ";
        var affectedRows = await connection.ExecuteAsync(sql, product);
        return affectedRows > 0;
    }

    public async Task<bool> Delete(long id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM products WHERE id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
}

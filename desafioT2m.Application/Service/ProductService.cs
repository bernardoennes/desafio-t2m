using desafioT2m.Domain;
using desafioT2m.Dto;
using desafio_t2m.Utils;

namespace desafio_t2m.Service;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly RabbitMQProducer _rabbitProducer;

    public ProductService(IProductRepository repository, RabbitMQProducer rabbitProducer)
    {
        _repository = repository;
        _rabbitProducer = rabbitProducer;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProducts()
    {
        var products = await _repository.GetAll();
        return products.Select(p => new ProductDTO
        {
            Name = p.Name,
            Quantity = p.Quantity,
            Description = p.Description,
            Price = p.Price
        });
    }

    public async Task<ProductDTO?> GetProductByName(string name)
    {
        var product = await _repository.GetByName(name);
        if (product is null) return null;
        return new ProductDTO
        {
            Name = product.Name,
            Quantity = product.Quantity,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task AddProduct(ProductDTO productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.Name))
            throw new ArgumentException("O Nome do Produto não pode estar vazio.");

        var product = new Product(
            productDto.Name,
            productDto.Quantity,
            productDto.Description,
            productDto.Price
        );

        await _repository.Add(product);

        _rabbitProducer.Publish(new
        {
            Action = "created",
            Product = productDto
        }, "product.created");

        if (productDto.Quantity < 100)
        {
            var status = productDto.Quantity < 10 ? "Crítico" : "Baixo";
            _rabbitProducer.Publish(new
            {
                Event = "Alerta de Estoque",
                ProductName = productDto.Name,
                Status = status,
                Quantity = productDto.Quantity
            }, "product.stockalert");
        }
    }

    public async Task UpdateProduct(string name, ProductDTO productDto)
    {
        var existing = await _repository.GetByName(name);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        var normalizedNewName = NameNormalizer.Normalize(productDto.Name);
        var existingProduct = await _repository.GetByName(productDto.Name);

        if (existingProduct != null && existingProduct.Id != existing.Id)
            throw new InvalidOperationException("Já existe um produto com esse nome.");

        existing.Name = productDto.Name;
        existing.NormalizedName = normalizedNewName;
        existing.Quantity = productDto.Quantity;
        existing.Description = productDto.Description;
        existing.Price = productDto.Price;

        await _repository.Update(existing);

        _rabbitProducer.Publish(new
        {
            Action = "updated",
            Product = productDto
        }, "product.updated");

        if (productDto.Quantity < 100)
        {
            var status = productDto.Quantity < 10 ? "Crítico" : "Baixo";
            _rabbitProducer.Publish(new
            {
                Event = "Alerta de Estoque",
                ProductName = productDto.Name,
                Status = status,
                Quantity = productDto.Quantity
            }, "product.stockalert");
        }
    }

    public async Task DeleteProduct(string name)
    {
        var existing = await _repository.GetByName(name);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        await _repository.Delete(existing.Id);

        _rabbitProducer.Publish(new
        {
            Action = "deleted",
            Product = new ProductDTO
            {
                Name = existing.Name,
                Quantity = existing.Quantity,
                Description = existing.Description,
                Price = existing.Price
            }
        }, "product.deleted");
    }
}

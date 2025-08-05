using desafioT2m.Domain;
using desafioT2m.Dto;
using desafioT2m.Infraestructure.RabbitMQ;

namespace desafioT2m.Service;

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

    public async Task<ProductDTO?> GetProductByBarCode(string barCode)
    {
        var product = await _repository.GetByBarCode(barCode);
        if (product is null) return null;
        return new ProductDTO
        {
            BarCode = product.BarCode,
            Name = product.Name,
            Quantity = product.Quantity,
            Description = product.Description,
            Price = product.Price
        };
    }

    public async Task AddProduct(ProductDTO productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.BarCode))
            throw new ArgumentException("O código de barras não pode estar vazio.");

        var product = new Product(
            productDto.BarCode,
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

    public async Task UpdateProduct(string barCode, ProductDTO productDto)
    {
        var existing = await _repository.GetByBarCode(barCode);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        existing.BarCode = productDto.BarCode;
        existing.Name = productDto.Name;
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

    public async Task DeleteProduct(string barCode)
    {
        var existing = await _repository.GetByBarCode(barCode);
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

using desafioT2m.Domain;
using desafioT2m.Dto;
using desafioT2m.Infraestructure.RabbitMQ;

namespace desafioT2m.Service;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IRabbitMQProducer _rabbitProducer;

    public ProductService(IProductRepository repository, IRabbitMQProducer rabbitProducer)
    {
        _repository = repository;
        _rabbitProducer = rabbitProducer;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProducts()
    {
        var products = await _repository.GetAll();
        return products.Select(p => new ProductDTO
        {
            barcode = p.barcode,
            name = p.name,
            quantity = p.quantity,
            description = p.description,
            price = p.price
        });
    }

    public async Task<ProductDTO?> GetProductByBarCode(string barCode)
    {
        var product = await _repository.GetByBarCode(barCode);
        if (product is null) return null;
        return new ProductDTO
        {
            barcode = product.barcode,
            name = product.name,
            quantity = product.quantity,
            description = product.description,
            price = product.price
        };
    }

    public async Task AddProduct(ProductDTO productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.barcode))
            throw new ArgumentException("O código de barras não pode estar vazio.");

        var product = new Product(
            productDto.barcode,
            productDto.name,
            productDto.quantity,
            productDto.description,
            productDto.price
        );

        await _repository.Add(product);

        _rabbitProducer.Publish(new
        {
            Action = "created",
            Product = productDto
        }, "product.created");

        if (productDto.quantity < 100)
        {
            var status = productDto.quantity < 10 ? "Crítico" : "Baixo";
            _rabbitProducer.Publish(new
            {
                Event = "Alerta de Estoque",
                ProductName = productDto.name,
                Status = status,
                Quantity = productDto.quantity
            }, "product.stockalert");
        }
    }

    public async Task UpdateProduct(string barCode, ProductDTO productDto)
    {
        var existing = await _repository.GetByBarCode(barCode);
        if (existing is null)
            throw new InvalidOperationException("O Produto informado não foi encontrado.");

        if (!string.Equals(barCode, productDto.barcode, StringComparison.OrdinalIgnoreCase))
        {
            var other = await _repository.GetByBarCode(productDto.barcode);
            if (other != null)
                throw new InvalidOperationException("Já existe outro produto com esse código de barras.");
        }
        existing.barcode = productDto.barcode;
        existing.name = productDto.name;
        existing.quantity = productDto.quantity;
        existing.description = productDto.description;
        existing.price = productDto.price;

        await _repository.Update(existing);

        _rabbitProducer.Publish(new
        {
            Action = "updated",
            Product = productDto
        }, "product.updated");

        if (productDto.quantity < 100)
        {
            var status = productDto.quantity < 10 ? "Crítico" : "Baixo";
            _rabbitProducer.Publish(new
            {
                Event = "Alerta de Estoque",
                ProductName = productDto.name,
                Status = status,
                Quantity = productDto.quantity
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
                name = existing.name,
                quantity = existing.quantity,
                description = existing.description,
                price = existing.price
            }
        }, "product.deleted");
    }
}

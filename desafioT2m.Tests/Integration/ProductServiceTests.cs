using desafioT2m.Domain;
using desafioT2m.Dto;
using desafioT2m.Service;
using desafioT2m.Infraestructure.RabbitMQ;
using Moq;

namespace desafio_t2m.ServiceTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly Mock<IRabbitMQProducer> _mockProducer;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _mockProducer = new Mock<IRabbitMQProducer>();
        _service = new ProductService(_mockRepo.Object, _mockProducer.Object);
    }

    [Fact]
    public async Task AddNewProduct()
    {
        var dto = new ProductDTO
        {
            barcode = "1234-5678",
            name = "Caneta",
            quantity = 5,
            description = "Caneta azul",
            price = 2.5M
        };

        await _service.AddProduct(dto);

        _mockRepo.Verify(r => r.Add(It.Is<Product>(p => p.barcode == dto.barcode)), Times.Once);
        _mockProducer.Verify(p => p.Publish(It.IsAny<object>(), "product.created"), Times.Once);
        _mockProducer.Verify(p => p.Publish(It.IsAny<object>(), "product.stockalert"), Times.Once);
    }

    [Fact]
    public async Task GetAllProducts()
    {
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Product>
        {
            new Product("1111-2222", "Produto 1", 10, "Desc", 5),
            new Product("3333-4444", "Produto 2", 20, "Desc", 15)
        });

        var result = await _service.GetAllProducts();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateProduct()
    {
        var dto = new ProductDTO
        {
            barcode = "1111-1111",
            name = "Produto Atualizado",
            quantity = 3,
            description = "Atualizado",
            price = 20
        };

        _mockRepo.Setup(r => r.GetByBarCode("1111-1111"))
            .ReturnsAsync(new Product("1111-1111", "Old", 1, "Old Desc", 10));

        await _service.UpdateProduct("1111-1111", dto);

        _mockRepo.Verify(r => r.Update(It.Is<Product>(p => p.name == dto.name)), Times.Once);
        _mockProducer.Verify(p => p.Publish(It.IsAny<object>(), "product.updated"), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct()
    {
        var product = new Product("2222-2222", "Produto", 1, "Desc", 9) { Id = 10 };

        _mockRepo.Setup(r => r.GetByBarCode("2222-2222"))
            .ReturnsAsync(product);

        await _service.DeleteProduct("2222-2222");

        _mockRepo.Verify(r => r.Delete(product.Id), Times.Once);
        _mockProducer.Verify(p => p.Publish(It.IsAny<object>(), "product.deleted"), Times.Once);
    }

    [Fact]
    public async Task LowStockAlert()
    {
        var dto = new ProductDTO
        {
            barcode = "9999-9999",
            name = "Produto Crítico",
            quantity = 2,
            description = "Estoque crítico",
            price = 10
        };

        await _service.AddProduct(dto);

        _mockProducer.Verify(static p => p.Publish(It.Is<object>(o => o.ToString().Contains("Crítico")), "product.stockalert"), Times.Once);
    }
}

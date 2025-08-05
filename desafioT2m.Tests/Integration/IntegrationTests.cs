using desafio_t2m;
using desafioT2m.Dto;
using desafioT2m.Infraestructure.RabbitMQ;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace desafio_t2m.Tests;

public class ProductTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public ProductTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables();
        _config = builder.Build();
    }

    private string barcodeGenerator()
    {
        var random = new Random();
        var part1 = random.Next(1000, 9999);
        var part2 = random.Next(1000, 9999);
        return $"{part1}-{part2}";
    }

    [Fact]
    public async Task listAllTest()
    {
        var response = await _client.GetAsync("/estoque");
        response.EnsureSuccessStatusCode();

        var produtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        produtos.Should().NotBeNull();
    }

    [Fact]
    public async Task AddTest()
    {
        var dto = new ProductDTO
        {
            barcode = barcodeGenerator(),
            name = "Caneta Arco-Iris",
            description = "testando o XUnit",
            price = 10,
            quantity = 1
        };
        var response = await _client.PostAsJsonAsync("/estoque", dto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task updateTest()
    {
        var barcode = barcodeGenerator();

        var dto = new ProductDTO
        {
            barcode = barcode,
            name = "Caneta Arco-Iris",
            description = "testando o XUnit",
            price = 10,
            quantity = 1
        };
        await _client.PostAsJsonAsync("/estoque", dto);

        var updatedDto = new ProductDTO
        {
            barcode = barcode,
            name = "Caneta Arco-Iris",
            description = "atualizado",
            price = 20,
            quantity = 5
        };
        var response = await _client.PutAsJsonAsync($"/estoque/{barcode}", updatedDto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task deleteTest()
    {
        var barcode = barcodeGenerator();

        var dto = new ProductDTO
        {
            barcode = barcode,
            name = "Caneta Arco-Iris",
            description = "para deletar",
            price = 15,
            quantity = 3
        };
        await _client.PostAsJsonAsync("/estoque", dto);

        var response = await _client.DeleteAsync($"/estoque/{barcode}");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void rabbitSendMessageTest()
    {
        var connection = new RabbitMQConnection(_config);
        var channel = connection.GetChannel();

        var mensagem = "Mensagem de teste RabbitMQ";

        var body = Encoding.UTF8.GetBytes(mensagem);
        var queueName = _config["RabbitMQ:QueueName"];

        Action act = () => channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: null,
            body: body
        );

        act.Should().NotThrow();
    }
}

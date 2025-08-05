using desafio_t2m;
using desafioT2m.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

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
        var dto = new ProductDTO { 
            Name = "Caneta Arco-Iris", 
            Description = "testando o NUnit", 
            Price = 10, 
            Quantity = 1 
        };
        var response = await _client.PostAsJsonAsync("/estoque", dto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task updateTest()
    {
        var dto = new ProductDTO { 
            Name = "Caneta Arco-Iris", 
            Description = "testando atualizar o NUnit", 
            Price = 20, 
            Quantity = 5 
        };
        var response = await _client.PutAsJsonAsync("/estoque/canetaarcoiris", dto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task deleteTest()
    {
        var response = await _client.DeleteAsync("/estoque/canetaarcoiris");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void rabbitProcessingTest()
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:HostName"],
            UserName = _config["RabbitMQ:UserName"],
            Password = _config["RabbitMQ:Password"],
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var queueName = "test-products-queue-" + Guid.NewGuid();
        var exchange = "product_exchange";
        var routingKey = "product.stockalert";

        channel.QueueDeclare(queue: queueName, durable: false, exclusive: true, autoDelete: true, arguments: null);
        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true);
        channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

        var mensagem = new
        {
            Event = "EstoqueAlerta",
            ProductName = "Teste XUnit",
            Status = "Crítico",
            Quantity = 5
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));
        channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);

        var consumer = new EventingBasicConsumer(channel);
        string? receivedMessage = null;
        var resetEvent = new ManualResetEvent(false);

        consumer.Received += (model, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(msg);
            if (json != null && json.ContainsKey("Event") && json["Event"]?.ToString() == "EstoqueAlerta")
            {
                receivedMessage = msg;
                resetEvent.Set();
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        resetEvent.WaitOne(TimeSpan.FromSeconds(10));

        receivedMessage.Should().NotBeNull();
        receivedMessage.Should().Contain("EstoqueAlerta");
        receivedMessage.Should().Contain("Teste XUnit");
        var json = JsonSerializer.Deserialize<Dictionary<string, object>>(receivedMessage);
        json.Should().NotBeNull();
        json!["Status"].ToString().Should().Be("Crítico");
    }
}

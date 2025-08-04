using desafio_t2m;
using desafio_t2m.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using Xunit;

namespace desafio_t2m.Tests;

public class ProductTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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
    public async Task listAllTest()
    {
        var response = await _client.GetAsync("/estoque");
        response.EnsureSuccessStatusCode();

        var produtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        produtos.Should().NotBeNull();
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
}

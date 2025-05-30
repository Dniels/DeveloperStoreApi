using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit
{
    public class SalesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SalesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database
                    services.AddDbContext<DefaultContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateSale_ValidRequest_ShouldReturnCreatedSale()
        {
            var request = new CreateSaleRequest
            {
                SaleNumber = "SALE-INT-001",
                Customer = new CustomerRequest
                {
                    Id = Guid.NewGuid(),
                    Name = "Integration Test",
                    Email = "test@test.com"
                },
                Branch = new BranchRequest
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Branch",
                    Address = "Test Address"
                },
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest
                    {
                        Product = new ProductRequest
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Product",
                            Description = "Test",
                            Category = "Category"
                        },
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/sales", request);

            // Debug - Adicionar logs se der erro
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var sale = JsonSerializer.Deserialize<SaleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(sale);
            Assert.Equal(request.SaleNumber, sale.SaleNumber);
            Assert.Equal(45.0m, sale.TotalAmount); // 50 - 10% discount
        }

        [Fact]
        public async Task GetSale_ExistingSale_ShouldReturnSale()
        {
            // Arrange - First create a sale
            var createRequest = new CreateSaleRequest
            {
                SaleNumber = "SALE-INT-002",
                Customer = new CustomerRequest
                {
                    Id = Guid.NewGuid(),
                    Name = "Integration Test",
                    Email = "test@test.com"
                },
                Branch = new BranchRequest
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Branch",
                    Address = "Test Address"
                },
                Items = new List<SaleItemRequest>
                {
                    new SaleItemRequest
                    {
                        Product = new ProductRequest
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Product",
                            Description = "Test",
                            Category = "Category"
                        },
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);

            // Debug - Adicionar logs se der erro
            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Create Status: {createResponse.StatusCode}");
                Console.WriteLine($"Create Error: {errorContent}");
            }

            createResponse.EnsureSuccessStatusCode();

            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdSale = JsonSerializer.Deserialize<SaleDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Act
            var getResponse = await _client.GetAsync($"/api/sales/{createdSale.Id}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var retrievedSale = JsonSerializer.Deserialize<SaleDto>(getContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(retrievedSale);
            Assert.Equal(createdSale.Id, retrievedSale.Id);
            Assert.Equal(createdSale.SaleNumber, retrievedSale.SaleNumber);
        }
    }
}
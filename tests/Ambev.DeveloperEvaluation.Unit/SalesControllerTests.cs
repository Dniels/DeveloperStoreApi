using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
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
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<DefaultContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        #region Create Sale Tests

        [Fact]
        public async Task CreateSale_ValidRequest_ShouldReturnCreatedSale()
        {
            var request = CreateValidSaleRequest("SALE-INT-001");

            var response = await _client.PostAsJsonAsync("/api/sales", request);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var sale = JsonSerializer.Deserialize<SaleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(sale);
            Assert.Equal(request.SaleNumber, sale.SaleNumber);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateSale_InvalidRequest_ShouldReturnBadRequest()
        {
            var request = new CreateSaleRequest(); // Empty request

            var response = await _client.PostAsJsonAsync("/api/sales", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Get Sale Tests

        [Fact]
        public async Task GetSale_ExistingSale_ShouldReturnSale()
        {
            var createRequest = CreateValidSaleRequest("SALE-INT-002");
            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdSale = JsonSerializer.Deserialize<SaleDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var getResponse = await _client.GetAsync($"/api/sales/{createdSale.Id}");

            getResponse.EnsureSuccessStatusCode();
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var retrievedSale = JsonSerializer.Deserialize<SaleDto>(getContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(retrievedSale);
            Assert.Equal(createdSale.Id, retrievedSale.Id);
            Assert.Equal(createdSale.SaleNumber, retrievedSale.SaleNumber);
        }

        [Fact]
        public async Task GetSale_NonExistingSale_ShouldReturnNotFound()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.GetAsync($"/api/sales/{nonExistingId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Get Sales (List) Tests

        [Fact]
        public async Task GetSales_WithoutParameters_ShouldReturnPagedResult()
        {
            // Create a sale first
            var createRequest = CreateValidSaleRequest("SALE-INT-003");
            await _client.PostAsJsonAsync("/api/sales", createRequest);

            var response = await _client.GetAsync("/api/sales");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetSales_WithPaginationParameters_ShouldReturnFilteredResult()
        {
            var response = await _client.GetAsync("/api/sales?_page=1&_size=5&customerName=Test");

            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Update Sale Tests

        //TODO: verificar testes
        //[Fact]
        //public async Task UpdateSale_ExistingSale_ShouldReturnUpdatedSale()
        //{
        //    // Create a sale first
        //    var createRequest = CreateValidSaleRequest("SALE-INT-004");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createdSale = JsonSerializer.Deserialize<SaleDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var updateRequest = new UpdateSaleRequest
        //    {
        //        Items = new List<SaleItemRequest>
        //        {
        //            new SaleItemRequest
        //            {
        //                Product = new ProductRequest
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Name = "Updated Product",
        //                    Description = "Updated",
        //                    Category = "Updated Category"
        //                },
        //                Quantity = 3,
        //                UnitPrice = 15.0m
        //            }
        //        }
        //    };

        //    var response = await _client.PutAsJsonAsync($"/api/sales/{createdSale.Id}", updateRequest);

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task UpdateSale_NonExistingSale_ShouldReturnNotFound()
        //{
        //    var nonExistingId = Guid.NewGuid();
        //    var updateRequest = new UpdateSaleRequest
        //    {
        //        Items = new List<SaleItemRequest>()
        //    };

        //    var response = await _client.PutAsJsonAsync($"/api/sales/{nonExistingId}", updateRequest);

        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        [Fact]
        public async Task UpdateSale_InvalidRequest_ShouldReturnBadRequest()
        {
            var existingId = Guid.NewGuid();
            var invalidRequest = new UpdateSaleRequest(); // Empty request

            var response = await _client.PutAsJsonAsync($"/api/sales/{existingId}", invalidRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Cancel Sale Tests

        [Fact]
        public async Task CancelSale_ExistingSale_ShouldReturnOk()
        {
            // Create a sale first
            var createRequest = CreateValidSaleRequest("SALE-INT-005");
            var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var createdSale = JsonSerializer.Deserialize<SaleDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.DeleteAsync($"/api/sales/{createdSale.Id}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("cancelled successfully", content);
        }

        [Fact]
        public async Task CancelSale_NonExistingSale_ShouldReturnNotFound()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.DeleteAsync($"/api/sales/{nonExistingId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        // TODO: verificar testes
        //#region Cancel Sale Item Tests

        //[Fact]
        //public async Task CancelSaleItem_ExistingSaleAndItem_ShouldReturnOk()
        //{
        //    // Create a sale first
        //    var createRequest = CreateValidSaleRequest("SALE-INT-006");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createdSale = JsonSerializer.Deserialize<SaleDto>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var productId = createRequest.Items.First().Product.Id;

        //    var response = await _client.DeleteAsync($"/api/sales/{createdSale.Id}/items/{productId}");

        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadAsStringAsync();

        //    Assert.Contains("item cancelled successfully", content);
        //}

        //[Fact]
        //public async Task CancelSaleItem_NonExistingSale_ShouldReturnNotFound()
        //{
        //    var nonExistingSaleId = Guid.NewGuid();
        //    var nonExistingProductId = Guid.NewGuid();

        //    var response = await _client.DeleteAsync($"/api/sales/{nonExistingSaleId}/items/{nonExistingProductId}");

        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        //#endregion

        #region Helper Methods

        private CreateSaleRequest CreateValidSaleRequest(string saleNumber)
        {
            return new CreateSaleRequest
            {
                SaleNumber = saleNumber,
                Customer = new CustomerRequest
                {
                    Id = Guid.NewGuid(),
                    Name = "Integration Test Customer",
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
                            Description = "Test Description",
                            Category = "Test Category"
                        },
                        Quantity = 5,
                        UnitPrice = 10.0m
                    }
                }
            };
        }

        #endregion
    }
}
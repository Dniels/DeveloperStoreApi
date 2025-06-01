using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
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

        #region Helper Methods

        /// <summary>
        /// Creates a valid sale request for testing purposes
        /// </summary>
        /// <param name="saleNumber">The sale number to use</param>
        /// <returns>A valid CreateSaleRequest object</returns>
        private CreateSaleRequest CreateValidSaleRequest(string saleNumber)
        {
            return new CreateSaleRequest
            {
                SaleNumber = saleNumber,
                Customer = new CustomerInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Customer",
                    Email = "test.customer@example.com"
                },
                Branch = new BranchInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Branch",
                    Address = "123 Test Street, Test City, TC 12345"
                },
                Items = new List<SaleItemInfo>
                {
                    new SaleItemInfo
                    {
                        Product = new ProductInfo
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Product",
                            Description = "Test Product Description",
                            Category = "Test Category"
                        },
                        Quantity = 2,
                        UnitPrice = 10.50m
                    }
                }
            };
        }

        #endregion

        #region Create Sale Tests

        //[Fact]
        //public async Task CreateSale_ValidRequest_ShouldReturnCreatedSale()
        //{
        //    var request = CreateValidSaleRequest("SALE-INT-001");

        //    var response = await _client.PostAsJsonAsync("/api/sales", request);

        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadAsStringAsync();
        //    var apiResponse = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    Assert.NotNull(apiResponse);
        //    Assert.True(apiResponse.Success);
        //    Assert.NotNull(apiResponse.Data);
        //    Assert.Equal(request.SaleNumber, apiResponse.Data.SaleNumber);
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}

        [Fact]
        public async Task CreateSale_InvalidRequest_ShouldReturnBadRequest()
        {
            var request = new CreateSaleRequest(); // Empty request

            var response = await _client.PostAsJsonAsync("/api/sales", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Get Sale Tests

        //[Fact]
        //public async Task GetSale_ExistingSale_ShouldReturnSale()
        //{
        //    var createRequest = CreateValidSaleRequest("SALE-INT-002");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createApiResponse = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResponse>>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var getResponse = await _client.GetAsync($"/api/sales/{createApiResponse.Data.Id}");

        //    getResponse.EnsureSuccessStatusCode();
        //    var getContent = await getResponse.Content.ReadAsStringAsync();
        //    var getApiResponse = JsonSerializer.Deserialize<ApiResponseWithData<GetSaleResponse>>(getContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    Assert.NotNull(getApiResponse);
        //    Assert.True(getApiResponse.Success);
        //    Assert.NotNull(getApiResponse.Data);
        //    Assert.Equal(createApiResponse.Data.Id, getApiResponse.Data.Id);
        //    Assert.Equal(createApiResponse.Data.SaleNumber, getApiResponse.Data.SaleNumber);
        //}

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
            var response = await _client.GetAsync("/api/sales");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetSales_WithPaginationParameters_ShouldReturnFilteredResult()
        {
            var response = await _client.GetAsync("/api/sales?page=1&size=5&customerName=Test");

            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Update Sale Tests

        //[Fact]
        //public async Task UpdateSale_ExistingSale_ShouldReturnUpdatedSale()
        //{
        //    // Create a sale first
        //    var createRequest = CreateValidSaleRequest("SALE-INT-004");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createApiResponse = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResponse>>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var updateRequest = new WebApi.Features.Sales.UpdateSale.UpdateSaleRequest
        //    {
        //        Items = new List<UpdateSaleItemRequest>
        //        {
        //            new UpdateSaleItemRequest
        //            {
        //                Product = new UpdateProductRequest
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Name = "Updated Product",
        //                    Description = "Updated Description",
        //                    Category = "Updated Category"
        //                },
        //                Quantity = 3,
        //                UnitPrice = 15.0m
        //            }
        //        }
        //    };

        //    var response = await _client.PutAsJsonAsync($"/api/sales/{createApiResponse.Data.Id}", updateRequest);

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task UpdateSale_NonExistingSale_ShouldReturnNotFound()
        //{
        //    var nonExistingId = Guid.NewGuid();
        //    var updateRequest = new WebApi.Features.Sales.UpdateSale.UpdateSaleRequest
        //    {
        //        Items = new List<UpdateSaleItemRequest>()
        //    };

        //    var response = await _client.PutAsJsonAsync($"/api/sales/{nonExistingId}", updateRequest);

        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        [Fact]
        public async Task UpdateSale_InvalidRequest_ShouldReturnBadRequest()
        {
            var existingId = Guid.NewGuid();
            var invalidRequest = new WebApi.Features.Sales.UpdateSale.UpdateSaleRequest(); // Empty request

            var response = await _client.PutAsJsonAsync($"/api/sales/{existingId}", invalidRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Cancel Sale Tests

        //[Fact]
        //public async Task CancelSale_ExistingSale_ShouldReturnOk()
        //{
        //    // Create a sale first
        //    var createRequest = CreateValidSaleRequest("SALE-INT-005");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createApiResponse = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResponse>>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var response = await _client.DeleteAsync($"/api/sales/{createApiResponse.Data.Id}");

        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadAsStringAsync();
        //    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    Assert.NotNull(apiResponse);
        //    Assert.True(apiResponse.Success);
        //    Assert.Contains("cancelled successfully", apiResponse.Message);
        //}

        [Fact]
        public async Task CancelSale_NonExistingSale_ShouldReturnNotFound()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.DeleteAsync($"/api/sales/{nonExistingId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Cancel Sale Item Tests

        //[Fact]
        //public async Task CancelSaleItem_ExistingSaleAndItem_ShouldReturnOk()
        //{
        //    // Create a sale first
        //    var createRequest = CreateValidSaleRequest("SALE-INT-006");
        //    var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        //    var createContent = await createResponse.Content.ReadAsStringAsync();
        //    var createApiResponse = JsonSerializer.Deserialize<ApiResponseWithData<CreateSaleResponse>>(createContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var productId = createRequest.Items.First().Product.Id;

        //    var response = await _client.DeleteAsync($"/api/sales/{createApiResponse.Data.Id}/items/{productId}");

        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadAsStringAsync();
        //    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    Assert.NotNull(apiResponse);
        //    Assert.True(apiResponse.Success);
        //    Assert.Contains("item cancelled successfully", apiResponse.Message);
        //}

        //[Fact]
        //public async Task CancelSaleItem_NonExistingSale_ShouldReturnNotFound()
        //{
        //    var nonExistingSaleId = Guid.NewGuid();
        //    var nonExistingProductId = Guid.NewGuid();

        //    var response = await _client.DeleteAsync($"/api/sales/{nonExistingSaleId}/items/{nonExistingProductId}");

        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        #endregion
    }

    // Classes para deserialização das respostas da API
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ValidationErrorDetail>? Errors { get; set; }
    }

    public class ApiResponseWithData<T> : ApiResponse
    {
        public T? Data { get; set; }
    }

    public class ValidationErrorDetail
    {
        public string Detail { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
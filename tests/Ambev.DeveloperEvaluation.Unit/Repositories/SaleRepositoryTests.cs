using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Builders;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Repositories
{
    public class SaleRepositoryTests : IDisposable
    {
        private readonly DefaultContext _context;
        private readonly SaleRepository _repository;

        public SaleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DefaultContext(options);
            _repository = new SaleRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ValidSale_ShouldAddToDatabase()
        {
            // Arrange
            var sale = new SaleBuilder()
                .WithItem(Guid.NewGuid(), "Product 1", 2, 10.0m)
                .Build();

            // Act
            await _repository.AddAsync(sale);
            await _context.SaveChangesAsync();

            // Assert
            var savedSale = await _context.Sales.FirstOrDefaultAsync();
            Assert.NotNull(savedSale);
            Assert.Equal(sale.SaleNumber, savedSale.SaleNumber);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingSale_ShouldReturnSale()
        {
            // Arrange
            var sale = new SaleBuilder()
                .WithItem(Guid.NewGuid(), "Product 1", 2, 10.0m)
                .Build();

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(sale.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sale.Id, result.Id);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetPagedAsync_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var sales = new List<Sale>
            {
                new SaleBuilder().WithSaleNumber("SALE-001").Build(),
                new SaleBuilder().WithSaleNumber("SALE-002").Build(),
                new SaleBuilder().WithSaleNumber("SALE-003").Build()
            };

            _context.Sales.AddRange(sales);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Count());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

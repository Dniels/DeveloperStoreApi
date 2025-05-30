using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Builders;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Entities
{
    public class SaleTests
    {
        [Fact]
        public void Sale_Create_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var saleNumber = "SALE-001";
            var saleDate = DateTime.UtcNow;
            var customer = new Customer(new Guid(), "John Doe", "john@example.com");
            var branch = new Branch(new Guid(), "Main Branch", "Downtown");

            // Act
            var sale = new Sale(saleNumber, customer, branch);

            // Assert
            Assert.Equal(saleNumber, sale.SaleNumber);
            Assert.True(saleDate <= sale.SaleDate);
            Assert.Equal(customer, sale.Customer);
            Assert.Equal(branch, sale.Branch);
            Assert.Empty(sale.Items);
            Assert.Equal(0, sale.TotalAmount);
        }

        [Fact]
        public void AddItem_ValidItem_ShouldAddItemToSale()
        {
            // Arrange
            var sale = new SaleBuilder().Build();
            var product = new Product(new Guid(), "Product 1","Description", "Category");

            // Act
            sale.AddItem(product, 2, 10.0m);

            // Assert
            Assert.Single(sale.Items);
            Assert.Equal(20.0m, sale.TotalAmount);
        }

        [Fact]
        public void AddItem_QuantityAbove20_ShouldThrowException()
        {
            // Arrange
            var sale = new SaleBuilder().Build();
            var product = new Product(new Guid(), "Product 1", "Description", "Category"); ;

            // Act & Assert
            Assert.Throws<DomainException>(() => sale.AddItem(product, 21, 10.0m));
        }

        [Fact]
        public void AddItem_Quantity4To9_ShouldApply10PercentDiscount()
        {
            // Arrange
            var sale = new SaleBuilder().Build();
            var product = new Product(new Guid(), "Product 1", "Description", "Category");

            // Act
            sale.AddItem(product, 5, 10.0m);

            // Assert
            var item = sale.Items.First();
            Assert.Equal(5.0m, item.Discount); // 10% of 50
            Assert.Equal(45.0m, item.TotalAmount); // 50 - 5
        }

        [Fact]
        public void AddItem_Quantity10To20_ShouldApply20PercentDiscount()
        {
            // Arrange
            var sale = new SaleBuilder().Build();
            var product = new Product(new Guid(), "Product 1", "Description", "Category");

            // Act
            sale.AddItem(product, 15, 10.0m);

            // Assert
            var item = sale.Items.First();
            Assert.Equal(30.0m, item.Discount); // 20% of 150
            Assert.Equal(120.0m, item.TotalAmount); // 150 - 30
        }

        [Fact]
        public void Cancel_ActiveSale_ShouldCancelSale()
        {
            // Arrange
            var sale = new SaleBuilder().Build();

            // Act
            sale.Cancel();

            // Assert
            Assert.True(sale.Items.Count == 0);
        }


        [Fact]
        public void CancelItem_ValidItem_ShouldCancelItem()
        {
            // Arrange
            var sale = new SaleBuilder()
                .WithItem(new Guid(), "Product 1", 2, 10.0m)
                .Build();

            var itemId = sale.Items.First().Id;

            // Act
            sale.CancelItem(itemId);

            // Assert
            var item = sale.Items.First();
            Assert.True(item.IsCancelled); 
        }
    }
}


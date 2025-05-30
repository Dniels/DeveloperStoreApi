using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Unit.Builders
{
    public class SaleBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _saleNumber = "SALE-001";
        private DateTime _saleDate = DateTime.UtcNow;
        private Customer _customer = new Customer(Guid.NewGuid(), "John Doe", "john@example.com");
        private Branch _branch = new Branch(Guid.NewGuid(), "Main Branch", "Downtown");
        private Product _product = new Product(Guid.NewGuid(), "Product name", "Product description", "product Category");
        private List<SaleItem> _items = new();

        public SaleBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public SaleBuilder WithSaleNumber(string saleNumber)
        {
            _saleNumber = saleNumber;
            return this;
        }

        public SaleBuilder WithSaleDate(DateTime saleDate)
        {
            _saleDate = saleDate;
            return this;
        }

        public SaleBuilder WithCustomer(Customer customer)
        {
            _customer = customer;
            return this;
        }

        public SaleBuilder WithBranch(Branch branch)
        {
            _branch = branch;
            return this;
        }

        public SaleBuilder WithProduct(Product product)
        {
            _product = product;
            return this;
        }

        public SaleBuilder WithItem(Guid productId, string productName, int quantity, decimal unitPrice, decimal discount = 0)
        {
            var product = new Product(productId, productName,"descricao", "Category");
            var item = new SaleItem(productId, product, quantity, unitPrice);
            _items.Add(item);
            return this;
        }

        public Sale Build()
        {
            var sale = new Sale(_saleNumber, _customer, _branch);

            foreach (var item in _items)
            {
                sale.AddItem(item.Product, item.Quantity, item.UnitPrice);
            }

            return sale;
        }
    }
}

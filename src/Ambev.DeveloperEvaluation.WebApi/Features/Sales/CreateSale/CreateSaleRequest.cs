namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Request model for creating a new sale
    /// </summary>
    public class CreateSaleRequest
    {
        /// <summary>
        /// The sale number identifier
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Customer information for the sale
        /// </summary>
        public CustomerInfo Customer { get; set; } = new();

        /// <summary>
        /// Branch information where the sale is made
        /// </summary>
        public BranchInfo Branch { get; set; } = new();

        /// <summary>
        /// List of items being sold
        /// </summary>
        public List<SaleItemInfo> Items { get; set; } = new();
    }

    /// <summary>
    /// Customer information
    /// </summary>
    public class CustomerInfo
    {
        /// <summary>
        /// Customer unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customer email address
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Branch information
    /// </summary>
    public class BranchInfo
    {
        /// <summary>
        /// Branch unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Branch address
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sale item information
    /// </summary>
    public class SaleItemInfo
    {
        /// <summary>
        /// Product information
        /// </summary>
        public ProductInfo Product { get; set; } = new();

        /// <summary>
        /// Quantity of the product
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price of the product
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// Product information
    /// </summary>
    public class ProductInfo
    {
        /// <summary>
        /// Product unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Product category
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }
}
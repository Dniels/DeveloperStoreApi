namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// API response model for GetSale operation
    /// </summary>
    public class GetSaleResponse
    {
        /// <summary>
        /// The unique identifier of the sale
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The sale number
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// The sale date and time
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Customer information
        /// </summary>
        public CustomerResponse Customer { get; set; } = new();

        /// <summary>
        /// Branch information
        /// </summary>
        public BranchResponse Branch { get; set; } = new();

        /// <summary>
        /// List of sale items
        /// </summary>
        public List<SaleItemResponse> Items { get; set; } = new();

        /// <summary>
        /// Total amount of the sale
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Total discount applied
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Indicates if the sale is cancelled
        /// </summary>
        public bool IsCancelled { get; set; }
    }

    /// <summary>
    /// Customer response model
    /// </summary>
    public class CustomerResponse
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
        /// Customer email
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Branch response model
    /// </summary>
    public class BranchResponse
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
        public String Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sale item response model
    /// </summary>
    public class SaleItemResponse
    {
        /// <summary>
        /// Product information
        /// </summary>
        public ProductResponse Product { get; set; } = new();

        /// <summary>
        /// Quantity sold
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Discount applied to this item
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Total amount for this item (quantity * unit price - discount)
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Indicates if this item is cancelled
        /// </summary>
        public bool IsCancelled { get; set; }
    }

    /// <summary>
    /// Product response model
    /// </summary>
    public class ProductResponse
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

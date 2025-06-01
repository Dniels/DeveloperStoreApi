namespace Ambev.DeveloperEvaluation.WebApi.Models
{
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
        public string Address { get; set; } = string.Empty;
    }
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
}

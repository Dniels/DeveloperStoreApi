namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    /// <summary>
    /// Request model for updating an existing sale
    /// </summary>
    public class UpdateSaleRequest
    {
        /// <summary>
        /// List of items to update in the sale
        /// </summary>
        public List<UpdateSaleItemRequest> Items { get; set; } = new();
    }

    /// <summary>
    /// Sale item information for update
    /// </summary>
    public class UpdateSaleItemRequest
    {
        /// <summary>
        /// Product information
        /// </summary>
        public UpdateProductRequest Product { get; set; } = new();

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
    /// Product information for update
    /// </summary>
    public class UpdateProductRequest
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
using Ambev.DeveloperEvaluation.WebApi.Models;

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

    

   

    
    
}

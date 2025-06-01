namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CanceSale
{
    /// <summary>
    /// Request model for cancelling a sale
    /// </summary>
    public class CancelSaleRequest
    {
        /// <summary>
        /// The unique identifier of the sale to cancel
        /// </summary>
        public Guid Id { get; set; }
    }
}

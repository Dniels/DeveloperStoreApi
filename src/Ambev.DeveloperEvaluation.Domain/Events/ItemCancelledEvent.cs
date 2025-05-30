using Ambev.DeveloperEvaluation.Domain.Base;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public record ItemCancelledEvent(
        Guid SaleId,
        string SaleNumber,
        Guid ProductId
    ) : IDomainEvent;

}

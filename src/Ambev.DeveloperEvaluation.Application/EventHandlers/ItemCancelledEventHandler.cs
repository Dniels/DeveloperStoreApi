using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.EventHandlers
{
    public class ItemCancelledEventHandler : INotificationHandler<ItemCancelledEvent>
    {
        private readonly ILogger<ItemCancelledEventHandler> _logger;

        public ItemCancelledEventHandler(ILogger<ItemCancelledEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Item Cancelled Event: SaleId={SaleId}, SaleNumber={SaleNumber}, ProductId={ProductId}",
                notification.SaleId,
                notification.SaleNumber,
                notification.ProductId);

            // Here you could update inventory, notify warehouse, etc.
            return Task.CompletedTask;
        }
    }

}

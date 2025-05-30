using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.EventHandlers
{
    public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
    {
        private readonly ILogger<SaleModifiedEventHandler> _logger;

        public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Sale Modified Event: SaleId={SaleId}, SaleNumber={SaleNumber}",
                notification.SaleId,
                notification.SaleNumber);

            // Here you could publish to a message broker, update read models, etc.
            return Task.CompletedTask;
        }
    }
}

﻿using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers
{
    public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedEventHandler> _logger;

        public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Sale Created Event: SaleId={SaleId}, SaleNumber={SaleNumber}, CustomerId={CustomerId}, BranchId={BranchId}, Date={SaleDate}",
                notification.SaleId,
                notification.SaleNumber,
                notification.CustomerId,
                notification.BranchId,
                notification.SaleDate);

            return Task.CompletedTask;
        }
    }
}

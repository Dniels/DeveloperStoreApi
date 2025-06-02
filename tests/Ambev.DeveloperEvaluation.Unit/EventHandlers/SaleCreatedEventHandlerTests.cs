using Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.EventHandlers
{
    public class SaleCreatedEventHandlerTests
    {
        private readonly ILogger<SaleCreatedEventHandler> _logger;
        private readonly SaleCreatedEventHandler _handler;

        public SaleCreatedEventHandlerTests()
        {
            _logger = Substitute.For<ILogger<SaleCreatedEventHandler>>();
            _handler = new SaleCreatedEventHandler(_logger);
        }

        [Fact]
        public async Task Handle_SaleCreatedEvent_ShouldLogEvent()
        {
            // Arrange
            var saleCreatedEvent = new SaleCreatedEvent(Guid.NewGuid(), "SALE-001", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

            // Act
            await _handler.Handle(saleCreatedEvent, CancellationToken.None);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception?>(),
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}
using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.Handlers;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Commands
{
    public class CreateSaleCommandHandlerTests : TestBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;
        private readonly CreateSaleCommandHandler _handler;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public CreateSaleCommandHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _mediator = Substitute.For<IMediator>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _handler = new CreateSaleCommandHandler(_saleRepository, Mapper, _domainEventDispatcher);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateSale()
        {
            // Arrange
            var command = new CreateSaleCommand(
                "SALE-001",
                new Guid("19201289-b358-41ba-9633-034c6399664c"),
                "John Doe",
                "john@example.com",
                new Guid("5ec86aaf-c28a-47f4-84c1-1521c3788456"),
                "Main Branch",
                "Downtown",
                new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto(
                        new Guid("6946bc03-86af-4848-b62a-eea03ff4c7aa"),
                        "Product 1",
                        "Category",
                        "Description",
                        2,
                        10.0m)
                });

            _saleRepository.AddAsync(Arg.Any<Sale>()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.SaleNumber, result.SaleNumber);
            //await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>());
            //await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_InvalidQuantity_ShouldThrowException()
        {
            // Arrange
            var command = new CreateSaleCommand
            (
                "SALE-001",
                 Guid.NewGuid(),
                "John Doe",
                "john@example.com",
                Guid.NewGuid(),
                "Main Branch",
                "Downtown",
                new List<CreateSaleItemDto>
                {
                    new CreateSaleItemDto
                    (
                        Guid.NewGuid(),
                        "Product 1", 
                        "Description",
                        "Category",
                        25, // Invalid quantity
                        10.0m
                    )
                }
            );

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}

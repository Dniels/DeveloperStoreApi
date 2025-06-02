using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Handlers
{
    public class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public CancelSaleItemCommandHandler(
            ISaleRepository saleRepository,
            IDomainEventDispatcher eventDispatcher)
        {
            _saleRepository = saleRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
                throw new NotFoundException($"Sale with ID {request.SaleId} not found");

            var itemCancelled = sale.CancelItem(request.ProductId);
            if (!itemCancelled)
                throw new NotFoundException($"Product with ID {request.ProductId} not found in sale");

            await _saleRepository.UpdateAsync(sale, cancellationToken);
            await _eventDispatcher.DispatchEventsAsync(sale, cancellationToken);

            return true;
        }
    }
}

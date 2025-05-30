using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Handlers
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public CancelSaleCommandHandler(ISaleRepository saleRepository, IDomainEventDispatcher eventDispatcher)
        {
            _saleRepository = saleRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null)
                throw new NotFoundException($"Sale with ID {request.Id} not found");

            sale.Cancel();
            await _saleRepository.UpdateAsync(sale, cancellationToken);
            await _eventDispatcher.DispatchEventsAsync(sale, cancellationToken);

            return true;
        }
    }
}

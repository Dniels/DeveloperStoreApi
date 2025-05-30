using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Handlers
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public UpdateSaleCommandHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IDomainEventDispatcher eventDispatcher)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null)
                throw new NotFoundException($"Sale with ID {request.Id} not found");

            // Remove all existing items and add new ones
            var existingProducts = sale.Items.Select(x => x.Product.Id).ToList();
            foreach (var productId in existingProducts)
            {
                sale.RemoveItem(productId);
            }

            foreach (var item in request.Items)
            {
                var product = new Product(item.ProductId, item.ProductName, item.ProductDescription, item.ProductCategory);
                sale.AddItem(product, item.Quantity, item.UnitPrice);
            }

            await _saleRepository.UpdateAsync(sale, cancellationToken);
            await _eventDispatcher.DispatchEventsAsync(sale, cancellationToken);

            return _mapper.Map<SaleDto>(sale);
        }
    }
}

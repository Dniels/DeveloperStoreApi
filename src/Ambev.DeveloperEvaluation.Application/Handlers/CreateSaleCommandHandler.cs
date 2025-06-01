using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Services;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Handlers
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public CreateSaleCommandHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            IDomainEventDispatcher eventDispatcher)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var existingSale = await _saleRepository.GetBySaleNumberAsync(request.SaleNumber, cancellationToken);
            if (existingSale != null)
                throw new ApplicationException($"Sale with number {request.SaleNumber} already exists");

            var customer = new Customer(request.CustomerId, request.CustomerName, request.CustomerEmail);
            var branch = new Branch(request.BranchId, request.BranchName, request.BranchAddress);

            var sale = new Sale(request.SaleNumber, customer, branch);

            foreach (var item in request.Items)
            {
                var product = new Product(item.ProductId, item.ProductName, item.ProductDescription, item.ProductCategory);
                sale.AddItem(product, item.Quantity, item.UnitPrice);
            }

            await _saleRepository.AddAsync(sale, cancellationToken);
            await _eventDispatcher.DispatchEventsAsync(sale, cancellationToken);

            return _mapper.Map<SaleDto>(sale);
        }
    }
}

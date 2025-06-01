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
            // Carrega a venda existente com todos os relacionamentos
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null)
                throw new NotFoundException($"Sale with ID {request.Id} not found");

            // IMPORTANTE: Não remover e recriar itens, mas sim atualizar os existentes
            // e gerenciar as diferenças de forma mais cuidadosa

            // Identifica itens para remover (que não estão na nova lista)
            var newProductIds = request.Items.Select(i => i.ProductId).ToHashSet();
            var itemsToRemove = sale.Items.Where(item => !newProductIds.Contains(item.Product.Id)).ToList();

            foreach (var item in itemsToRemove)
            {
                sale.RemoveItem(item.Product.Id);
            }

            // Atualiza ou adiciona itens
            foreach (var requestItem in request.Items)
            {
                var existingItem = sale.Items.FirstOrDefault(item => item.Product.Id == requestItem.ProductId);
                
                if (existingItem != null)
                {
                    // Remove o item existente e adiciona o novo com as quantidades atualizadas
                    sale.RemoveItem(existingItem.Product.Id);
                }

                // Cria um novo produto ou reutiliza se já existe
                var product = new Product(requestItem.ProductId, requestItem.ProductName,
                                        requestItem.ProductDescription, requestItem.ProductCategory);
                sale.AddItem(product, requestItem.Quantity, requestItem.UnitPrice);
            }

            // Salva as mudanças
            await _saleRepository.UpdateAsync(sale, cancellationToken);

            // Despacha os eventos de domínio
            await _eventDispatcher.DispatchEventsAsync(sale, cancellationToken);

            return _mapper.Map<SaleDto>(sale);
        }
    }
}

using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales.Queries;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.Handlers
{
    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, PagedResult<SaleDto>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public GetSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var sales = await _saleRepository.GetAllAsync(request.Page, request.Size, cancellationToken);
            var totalItems = await _saleRepository.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalItems / request.Size);

            var salesDto = _mapper.Map<List<SaleDto>>(sales);

            return new PagedResult<SaleDto>
            {
                Data = salesDto,
                TotalItems = totalItems,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
    }
}

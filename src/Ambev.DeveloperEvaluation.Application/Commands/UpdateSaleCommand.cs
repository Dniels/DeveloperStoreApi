using Ambev.DeveloperEvaluation.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Commands
{
    public record UpdateSaleCommand(
        Guid Id,
        List<UpdateSaleItemDto> Items
    ) : IRequest<SaleDto>;

    public record UpdateSaleItemDto(
        Guid ProductId,
        string ProductName,
        string ProductDescription,
        string ProductCategory,
        int Quantity,
        decimal UnitPrice
    );

}

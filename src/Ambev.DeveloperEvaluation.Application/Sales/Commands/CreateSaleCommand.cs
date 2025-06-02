using Ambev.DeveloperEvaluation.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands
{
    public record CreateSaleCommand(
        string SaleNumber,
        Guid CustomerId,
        string CustomerName,
        string CustomerEmail,
        Guid BranchId,
        string BranchName,
        string BranchAddress,
        List<CreateSaleItemDto> Items
    ) : IRequest<SaleDto>;

    public record CreateSaleItemDto(
        Guid ProductId,
        string ProductName,
        string ProductDescription,
        string ProductCategory,
        int Quantity,
        decimal UnitPrice
    );

}

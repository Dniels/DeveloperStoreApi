using Ambev.DeveloperEvaluation.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Queries
{
    public record GetSalesQuery(
    int Page = 1,
    int Size = 10,
    string? OrderBy = null,
    string? CustomerName = null,
    string? BranchName = null,
    DateTime? MinDate = null,
    DateTime? MaxDate = null,
    bool? IsCancelled = null
) : IRequest<PagedResult<SaleDto>>;
}

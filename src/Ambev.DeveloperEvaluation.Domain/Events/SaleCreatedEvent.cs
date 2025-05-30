using Ambev.DeveloperEvaluation.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public record SaleCreatedEvent(
        Guid SaleId,
        string SaleNumber,
        Guid CustomerId,
        Guid BranchId,
        DateTime SaleDate
    ) : IDomainEvent;

}

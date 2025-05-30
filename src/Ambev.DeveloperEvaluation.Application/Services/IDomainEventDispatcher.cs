using Ambev.DeveloperEvaluation.Domain.Base;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Application.Services
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(BaseEntity entity, CancellationToken cancellationToken = default);
    }
}

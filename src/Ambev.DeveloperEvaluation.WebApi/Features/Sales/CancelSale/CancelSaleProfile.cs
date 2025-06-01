using Ambev.DeveloperEvaluation.Application.Commands;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    /// <summary>
    /// Profile for mapping CancelSale feature requests to commands
    /// </summary>
    public class CancelSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CancelSale feature
        /// </summary>
        public CancelSaleProfile()
        {
            CreateMap<Guid, CancelSaleCommand>()
                .ConstructUsing(id => new CancelSaleCommand(id));
        }
    }
}

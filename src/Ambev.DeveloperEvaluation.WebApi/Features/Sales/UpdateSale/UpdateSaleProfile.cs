using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    /// <summary>
    /// Profile for mapping between Application and API UpdateSale requests/responses
    /// </summary>
    public class UpdateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for UpdateSale feature
        /// </summary>
        public UpdateSaleProfile()
        {
            CreateMap<(Guid Id, UpdateSaleRequest Request), UpdateSaleCommand>()
                .ConstructUsing(src => new UpdateSaleCommand(
                    src.Id,
                    src.Request.Items.Select(i => new UpdateSaleItemDto(
                        i.Product.Id,
                        i.Product.Name,
                        i.Product.Description,
                        i.Product.Category,
                        i.Quantity,
                        i.UnitPrice
                    )).ToList()
                ));

            CreateMap<SaleDto, UpdateSaleResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new CustomerResponse
                {
                    Id = src.Customer.Id,
                    Name = src.Customer.Name,
                    Email = src.Customer.Email
                }))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new BranchResponse
                {
                    Id = src.Branch.Id,
                    Name = src.Branch.Name,
                    Address = src.Branch.Address
                }))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new SaleItemResponse
                {
                    Product = new ProductResponse
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        Category = i.Product.Category
                    },
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount,
                    TotalAmount = i.TotalAmount,
                    IsCancelled = i.IsCancelled
                }).ToList()));
        }
    }
}

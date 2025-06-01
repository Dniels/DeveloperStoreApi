using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.WebApi.Models;
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
            CreateMap<UpdateSaleItemRequest, UpdateSaleItemDto>()
                .ConstructUsing(src => new UpdateSaleItemDto(
                    src.Product.Id,
                    src.Product.Name,
                    src.Product.Description,
                    src.Product.Category,
                    src.Quantity,
                    src.UnitPrice
                ));

            CreateMap<(Guid Id, UpdateSaleRequest Request), UpdateSaleCommand>()
                .ConstructUsing((src, context) => new UpdateSaleCommand(
                    src.Id,
                    context.Mapper.Map<List<UpdateSaleItemDto>>(src.Request.Items)
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
                    Product = new Models.ProductResponse
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

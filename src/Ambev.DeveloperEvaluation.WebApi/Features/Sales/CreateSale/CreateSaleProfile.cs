using Ambev.DeveloperEvaluation.Application.Commands;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.WebApi.Models;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Profile for mapping between Application and API CreateSale requests/responses
    /// </summary>
    public class CreateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CreateSale feature
        /// </summary>
        public CreateSaleProfile()
        {
            CreateMap<CreateSaleRequest, CreateSaleCommand>()
                .ConstructUsing((src, context) => new CreateSaleCommand(
                    src.SaleNumber,
                    src.Customer.Id,
                    src.Customer.Name,
                    src.Customer.Email,
                    src.Branch.Id,
                    src.Branch.Name,
                    src.Branch.Address,
                    context.Mapper.Map<List<CreateSaleItemDto>>(src.Items) 
                ));

            CreateMap<SaleDto, CreateSaleResponse>()
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

            CreateMap<SaleItemInfo, CreateSaleItemDto>()
                .ConstructUsing(src => new CreateSaleItemDto(
                    src.Product.Id,
                    src.Product.Name,
                    src.Product.Description,
                    src.Product.Category,
                    src.Quantity,
                    src.UnitPrice
                ));

        }
    }
}

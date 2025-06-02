using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales.Queries;
using Ambev.DeveloperEvaluation.WebApi.Models;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Profile for mapping GetSale feature requests to commands
    /// </summary>
    public class GetSaleProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for GetSale feature
        /// </summary>
        public GetSaleProfile()
        {
            CreateMap<Guid, GetSaleByIdQuery>()
                .ConstructUsing(id => new GetSaleByIdQuery(id));

            CreateMap<SaleDto, GetSaleResponse>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
    .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber)) 
    .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate))
    .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
    .ForMember(dest => dest.TotalDiscount, opt => opt.MapFrom(src => src.Items.Sum(x => x.Discount)))
    .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.IsCancelled))
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

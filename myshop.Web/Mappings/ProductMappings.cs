using AutoMapper;
using MyShop.Business.DTOs;
using MyShop.Domain.Models;

namespace MyShop.Web.Mappings;

public class ProductMappings : Profile
{
    public ProductMappings()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name)
            );
    }
}
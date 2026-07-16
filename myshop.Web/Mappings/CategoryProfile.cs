using AutoMapper;
using MyShop.Business.DTOs;
using MyShop.Domain.Models;

namespace myshop.Web.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}

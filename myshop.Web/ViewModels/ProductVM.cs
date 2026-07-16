using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyShop.Business.DTOs;
using MyShop.Domain.Models;

namespace myshop.Entities.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = null!;
        [ValidateNever]
        public IEnumerable<CategoryDto> CategoryList { get; set; } = [];
    }
}

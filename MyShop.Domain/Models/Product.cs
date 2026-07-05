using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Domain.Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [DisplayName("Image")]
        [ValidateNever]
        public string Img { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace MyShop.Domain.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}

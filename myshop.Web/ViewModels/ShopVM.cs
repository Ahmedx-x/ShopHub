using MyShop.Domain.Models;

namespace myshop.Web.ViewModels
{
    public class ShopVM
    {
        public Pagination<Product> Products { get; set; } = new() { PageSize = 9 };
        public IReadOnlyList<Category> Categories { get; set; } = [];
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}

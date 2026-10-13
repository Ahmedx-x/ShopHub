using MyShop.Domain.Models;

namespace myshop.Web.ViewModels
{
    public class HomeVM
    {
        public IReadOnlyList<Category> Categories { get; set; } = [];
        public IReadOnlyList<Product> Products { get; set; } = [];
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
    }
}

using MyShop.Domain.Models;

namespace MyShop.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Pagination<Product>> SearchAsync(
            string searchTerm,
            int pageSize = 10,
            int pageNumber = 1);
    }
}

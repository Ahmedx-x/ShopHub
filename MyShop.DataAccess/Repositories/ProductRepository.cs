using MyShop.DataAccess.Context;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace MyShop.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context)
            : base(context)
        {

        }
    }
}

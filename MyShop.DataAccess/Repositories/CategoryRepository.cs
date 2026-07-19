using MyShop.DataAccess.Context;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace MyShop.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context)
            : base(context)
        {

        }
    }
}

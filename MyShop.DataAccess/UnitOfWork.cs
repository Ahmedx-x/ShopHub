using myshop.DataAccess.Context;
using MyShop.Domain.Interfaces;

namespace MyShop.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository Products { get; }

        public ICategoryRepository Categories { get; }

        public UnitOfWork(ApplicationDbContext context,
            IProductRepository products, ICategoryRepository categories)
        {
            _context = context;
            Products = products;
            Categories = categories;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

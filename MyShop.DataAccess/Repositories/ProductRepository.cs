using Microsoft.EntityFrameworkCore;
using MyShop.DataAccess.Context;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace MyShop.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Pagination<Product>> SearchAsync(
            string searchTerm,
            int pageSize = 10,
            int pageNumber = 1)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();

            pageSize = Math.Clamp(pageSize, 1, 10);
            pageNumber = Math.Max(pageNumber, 1);

            var items = await query
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

            return new Pagination<Product>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}

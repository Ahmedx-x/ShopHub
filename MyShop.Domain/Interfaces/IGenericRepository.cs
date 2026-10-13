using MyShop.Domain.Models;
using System.Linq.Expressions;

namespace MyShop.Domain.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<Pagination<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            string? include = null,
            int pageSize = 0,
            int pageNumber = 1);
        Task<T?> GetByIdAsync(int id);
        Task<T?> FindAsync(
            Expression<Func<T, bool>>? predicate = null,
            bool isTracked = true,
            string? include = null);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(int id);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}

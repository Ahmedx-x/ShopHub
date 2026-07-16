using System.Linq.Expressions;

namespace MyShop.Domain.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            string? include = null);
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

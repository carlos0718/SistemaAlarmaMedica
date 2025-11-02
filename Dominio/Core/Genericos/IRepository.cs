using System.Linq.Expressions;

namespace Dominio.Core.Genericos
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> GetByIdWithIncludesAsync(int id, string idPropertyName, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllAsyncWithIncludes(List<Expression<Func<TEntity, object>>> includes);
        Task<List<TEntity>> GetAllAsyncWithIncludesAndThen(Func<IQueryable<TEntity>, IQueryable<TEntity>> includes, Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}

using Dominio.Core.Genericos;
using Infraestructura.ContextoBD;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infraestructura.Genericos
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IAplicacionBDContexto _context;

        public Repository(IAplicacionBDContexto context)
        {
            _context = context;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> GetByIdWithIncludesAsync(
            int id,
            string idPropertyName,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null
            )
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(entity => EF.Property<int>(entity, idPropertyName) == id);

            return await query.FirstOrDefaultAsync();
        }


        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> GetAllAsyncWithIncludes(List<Expression<Func<TEntity, object>>> includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<List<TEntity>> GetAllAsyncWithIncludesAndThen(
            Func<IQueryable<TEntity>,
            IQueryable<TEntity>> includes,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null
            )
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = filter(query);
            }

            query = includes(query);
            return await query.ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Infraestructura.ContextoBD
{
    public interface IAplicacionBDContexto : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        Task CommitTransactionAsync();
        void RollbackTransaction();
    }
}

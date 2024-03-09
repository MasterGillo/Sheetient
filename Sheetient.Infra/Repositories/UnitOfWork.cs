using Sheetient.Domain.Interfaces;
using Sheetient.Infra.Data;

namespace Sheetient.Infra.Repositories
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly Dictionary<Type, object> _repositories = [];
        private bool _disposed;

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            var repository = new Repository<TEntity>(_dbContext);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;

        }
    }
}

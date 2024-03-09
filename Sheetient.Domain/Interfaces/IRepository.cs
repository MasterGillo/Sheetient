using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Sheetient.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> Get(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        Task<List<TEntity>> GetMany(Expression<Func<TEntity, bool>>? filter = null);
        Task Add(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}

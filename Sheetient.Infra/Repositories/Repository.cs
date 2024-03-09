using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Sheetient.Domain.Interfaces;
using System.Linq.Expressions;

namespace Sheetient.Infra.Repositories
{
    public class Repository<TEntity>(IApplicationDbContext dbContext) : IRepository<TEntity> where TEntity : class, IEntity
    {
        public async Task<TEntity?> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (include != null)
            {
                query = include(query);
            }
            return await query.SingleOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetMany(Expression<Func<TEntity, bool>>? filter = null)
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task Add(TEntity entity)
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
        }
    }
}

namespace Sheetient.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    }
}

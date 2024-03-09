using Microsoft.EntityFrameworkCore;

namespace Sheetient.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<T> Set<T>() where T : class;
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sheetient.Domain.Entities;
using Sheetient.Domain.Entities.Identity;
using Sheetient.Domain.Interfaces;
using System.Reflection;

namespace Sheetient.Infra.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options), IApplicationDbContext
    {
        public DbSet<Page> Pages { get; set; }
        public DbSet<Sheet> Sheets { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<LabelField> LabelFields { get; set; }
        public DbSet<TextInputField> TextInputFields { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sheetient.Domain.Entities.Identity;
using Sheetient.Domain.Interfaces;
using Sheetient.Infra.Data;
using Sheetient.Infra.Repositories;
using Sheetient.Infra.Settings;
using Sheetient.Infra.TokenProviders;

namespace Sheetient.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient);

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._+";
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<AccessTokenProvider<User>>(jwtSettings?.AccessTokenName ?? "accessToken")
            .AddTokenProvider<RefreshTokenProvider<User>>(jwtSettings?.RefreshTokenName ?? "refreshToken");

            return services;
        }
    }
}

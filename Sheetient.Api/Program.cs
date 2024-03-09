using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Sheetient.Api.Middleware;
using Sheetient.App;
using Sheetient.App.Services;
using Sheetient.App.Services.Interfaces;
using Sheetient.App.Settings;
using Sheetient.Infra;
using System.Text;

namespace Sheetient.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                var builtInFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = context =>
                {
                    if (!context.ModelState.IsValid)
                    {
                        LogAutomaticBadRequest(context);
                    }
                    return builtInFactory(context);
                };
            }).AddNewtonsoftJson();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            var jwtSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>();

            builder.Services.AddAuthorization();

            if (jwtSettings != null)
            {
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            }

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ISheetService, SheetService>();
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapControllers();

            app.Run();
        }

        private static void LogAutomaticBadRequest(ActionContext context)
        {
            var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName ?? string.Empty);

            var errorMessages = string.Join(" | ", context.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));

            var request = context.HttpContext.Request;

            logger.LogError("Validation Error." +
                            "\nError(s): {errorMessages}" +
                            "\n|{method}| Full URL: {path}{queryString}", errorMessages, request.Method, request.Path, request.QueryString);
        }
    }
}
using Microsoft.AspNetCore.Http;
using Sheetient.App.Exceptions;
using Sheetient.App.Services.Interfaces;
using System.Security.Claims;

namespace Sheetient.App.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get => Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public string UserName
        {
            get => _httpContextAccessor.HttpContext!.User.Identity?.Name ?? throw new UnauthorizedException("Invalid token.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sheetient.App.Dtos.Auth;
using Sheetient.App.Exceptions;
using Sheetient.App.Services.Interfaces;
using Sheetient.App.Settings;

namespace Sheetient.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthService authService, IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequestDto authRegisterRequestDto)
        {
            var response = await _authService.Register(authRegisterRequestDto);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            };
            if (response.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
            }
            HttpContext.Response.Cookies.Append(_jwtSettings.RefreshTokenName, response.RefreshToken, cookieOptions);
            return Ok(new AccessTokenResponseDto(response.AccessToken));
        }

        [HttpPost]
        [ProducesResponseType<AccessTokenResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequestDto authLoginRequestDto)
        {
            var response = await _authService.Login(authLoginRequestDto);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            };
            if (response.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
            }
            HttpContext.Response.Cookies.Append(_jwtSettings.RefreshTokenName, response.RefreshToken, cookieOptions);
            return Ok(new AccessTokenResponseDto(response.AccessToken));
        }

        [HttpPost]
        [ProducesResponseType<AccessTokenResponseDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[_jwtSettings.RefreshTokenName];
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ForbiddenException();
            }

            var response = await _authService.Refresh(refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            };
            if (response.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
            }
            HttpContext.Response.Cookies.Append(_jwtSettings.RefreshTokenName, response.RefreshToken, cookieOptions);
            return Ok(new AccessTokenResponseDto(response.AccessToken));
        }

        [HttpPost]
        public async Task<IActionResult> Revoke()
        {
            await _authService.Revoke();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sheetient.App.Dtos.Auth;
using Sheetient.App.Exceptions;
using Sheetient.App.Services.Interfaces;
using Sheetient.App.Settings;
using Sheetient.Domain.Entities.Identity;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace Sheetient.App.Services
{
    public class AuthService(
        UserManager<User> userManager,
        IOptionsSnapshot<JwtSettings> jwtSettings,
        IUserService userService) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        public async Task Register(AuthRegisterRequestDto authRegisterRequestDto)
        {
            var user = new User
            {
                UserName = authRegisterRequestDto.DisplayName,
                Email = authRegisterRequestDto.Email,
            };
            await userManager.CreateAsync(user, authRegisterRequestDto.Password);
        }

        public async Task<AuthLoginResponseDto> Login(AuthLoginRequestDto authLoginRequestDto)
        {
            var user =
                (MailAddress.TryCreate(authLoginRequestDto.UsernameOrEmail, out _)
                ? await userManager.FindByEmailAsync(authLoginRequestDto.UsernameOrEmail)
                : await userManager.FindByNameAsync(authLoginRequestDto.UsernameOrEmail))
                ?? throw new NotFoundException("Invalid credentials.");

            var validPassword = await userManager.CheckPasswordAsync(user, authLoginRequestDto.Password);
            if (!validPassword)
            {
                throw new NotFoundException("Invalid credentials.");
            }

            return await GenerateTokenResponse<AuthLoginResponseDto>(user);
        }

        public async Task<AuthRefreshResponseDto> Refresh(AuthRefreshRequestDto authRefreshRequestDto)
        {
            var username = await GetUsernameFromExpiredToken(authRefreshRequestDto.AccessToken);
            var user = (await userManager.FindByNameAsync(username))
                ?? throw new UnauthorizedException("Invalid token.");

            var verified = await userManager.VerifyUserTokenAsync(
                user,
                _jwtSettings.TokenProvider,
                _jwtSettings.Purpose,
                authRefreshRequestDto.RefreshToken);

            if (!verified)
            {
                throw new UnauthorizedException("Invalid token.");
            }

            return await GenerateTokenResponse<AuthRefreshResponseDto>(user);
        }

        public async Task Revoke()
        {
            var user = (await userManager.FindByNameAsync(userService.UserName))
                ?? throw new UnauthorizedException("Invalid token.");

            await userManager.RemoveAuthenticationTokenAsync(user, _jwtSettings.TokenProvider, _jwtSettings.Purpose);
        }

        private async Task<T> GenerateTokenResponse<T>(User user) where T : AuthTokenResponseBase, new()
        {
            var accessToken = await GenerateAccessToken(user);
            var refreshToken = await userManager.GenerateUserTokenAsync(user, _jwtSettings.TokenProvider, _jwtSettings.Purpose);

            await userManager.SetAuthenticationTokenAsync(user, _jwtSettings.TokenProvider, _jwtSettings.Purpose, refreshToken);

            return new T
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<string> GenerateAccessToken(User user)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName ?? string.Empty),
                new (JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userClaims);
            var roleClaims = userRoles.Select(x => new Claim(ClaimTypes.Role, x));
            claims.AddRange(roleClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        private async Task<string> GetUsernameFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
            var tokenHandler = new JsonWebTokenHandler();
            var validationResult = await tokenHandler.ValidateTokenAsync(accessToken, tokenValidationParameters);
            if (!validationResult.IsValid || validationResult.ClaimsIdentity.Name == null)
            {
                throw new UnauthorizedException("Invalid token.");
            }
            return validationResult.ClaimsIdentity.Name;
        }
    }
}

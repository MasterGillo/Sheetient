using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sheetient.Infra.Settings;
using System.Security.Claims;
using System.Text;
namespace Sheetient.Infra.TokenProviders
{
    public class RefreshTokenProvider<TUser>(IOptionsSnapshot<JwtSettings> jwtSettings) : IUserTwoFactorTokenProvider<TUser> where TUser : IdentityUser<int>
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(false);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName ?? string.Empty),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (ClaimTypes.IsPersistent, purpose.EndsWith("persistent").ToString(), ClaimValueTypes.Boolean)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };
            var tokenHandler = new JsonWebTokenHandler();
            return Task.FromResult(tokenHandler.CreateToken(tokenDescriptor));

        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
            };

            var tokenHandler = new JsonWebTokenHandler();
            var validationResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
            if (!validationResult.IsValid || validationResult.ClaimsIdentity.Name == null)
            {
                return false;
            }

            var currentRefreshToken = await manager.GetAuthenticationTokenAsync(user, "refreshToken", "refreshToken");
            var jwt = tokenHandler.ReadJsonWebToken(currentRefreshToken);
            var currentJti = jwt.GetClaim(JwtRegisteredClaimNames.Jti).Value;
            if (validationResult.Claims.TryGetValue(JwtRegisteredClaimNames.Jti, out var jti) && jti.ToString() != currentJti)
            {
                return false;
            }

            return true;
        }
    }
}

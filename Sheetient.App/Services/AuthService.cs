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
using System.Security.Cryptography;

namespace Sheetient.App.Services
{
    public class AuthService(
        UserManager<User> userManager,
        IOptionsSnapshot<JwtSettings> jwtSettings,
        IOptionsSnapshot<KeySettings> keySettings,
        IUserService userService) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly KeySettings _keySettings = keySettings.Value;
        public async Task Register(AuthRegisterRequestDto authRegisterRequestDto)
        {
            var user = new User
            {
                UserName = authRegisterRequestDto.DisplayName,
                Email = authRegisterRequestDto.Email,
            };
            await userManager.CreateAsync(user, authRegisterRequestDto.Password);
        }

        public async Task<AuthTokenResponseDto> Login(AuthLoginRequestDto authLoginRequestDto)
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

            return await GenerateTokenResponse(user, authLoginRequestDto.RememberMe);
        }

        public async Task<AuthTokenResponseDto> Refresh(string refreshToken)
        {
            var decryptedRefreshToken = DecryptToken(refreshToken);

            var tokenHandler = new JsonWebTokenHandler();
            var jwt = tokenHandler.ReadJsonWebToken(decryptedRefreshToken);
            var username = jwt.GetClaim(ClaimTypes.Name).Value;
            var isPersistent = jwt.GetClaim(ClaimTypes.IsPersistent).Value == true.ToString();
            var user = (await userManager.FindByNameAsync(username))
                ?? throw new UnauthorizedException("Invalid token.");

            await userManager.VerifyUserTokenAsync(user, _jwtSettings.RefreshTokenName, _jwtSettings.RefreshTokenName, decryptedRefreshToken);

            return await GenerateTokenResponse(user, isPersistent);
        }

        public async Task Revoke()
        {
            var user = (await userManager.FindByNameAsync(userService.UserName))
                ?? throw new UnauthorizedException("Invalid token.");

            await userManager.RemoveAuthenticationTokenAsync(user, _jwtSettings.RefreshTokenName, _jwtSettings.RefreshTokenName);
        }

        private async Task<AuthTokenResponseDto> GenerateTokenResponse(User user, bool rememberMe)
        {
            var accessToken = await userManager.GenerateUserTokenAsync(user, _jwtSettings.AccessTokenName, _jwtSettings.AccessTokenName);

            var refreshToken = await userManager.GenerateUserTokenAsync(user, _jwtSettings.RefreshTokenName, _jwtSettings.RefreshTokenName + (rememberMe ? "_persistent" : string.Empty));
            await userManager.SetAuthenticationTokenAsync(user, _jwtSettings.RefreshTokenName, _jwtSettings.RefreshTokenName, refreshToken);
            var encryptedRefreshToken = EncryptToken(refreshToken);
            return new AuthTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = encryptedRefreshToken,
                RememberMe = rememberMe
            };
        }

        private string EncryptToken(string innerJwt)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(_keySettings.PublicKey);
            var encryptingCredentials = new EncryptingCredentials
            (
                new RsaSecurityKey(rsa.ExportParameters(false)),
                SecurityAlgorithms.RsaOAEP,
                SecurityAlgorithms.Aes256CbcHmacSha512
            );
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.EncryptToken(innerJwt, encryptingCredentials);
        }

        private string DecryptToken(string token)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(_keySettings.PrivateKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                TokenDecryptionKey = new RsaSecurityKey(rsa)
            };
            var tokenHandler = new JsonWebTokenHandler();
            var jwt = new JsonWebToken(token);

            return tokenHandler.DecryptToken(jwt, tokenValidationParameters);
        }
    }
}

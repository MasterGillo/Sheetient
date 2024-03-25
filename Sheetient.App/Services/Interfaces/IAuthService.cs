using Sheetient.App.Dtos.Auth;

namespace Sheetient.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthTokenResponseDto> Register(AuthRegisterRequestDto authRegisterRequestDto);
        Task<AuthTokenResponseDto> Login(AuthLoginRequestDto authLoginRequestDto);
        Task<AuthTokenResponseDto> Refresh(string refreshToken);
        Task Revoke();
    }
}

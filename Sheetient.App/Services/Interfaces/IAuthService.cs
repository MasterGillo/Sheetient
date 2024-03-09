using Sheetient.App.Dtos.Auth;

namespace Sheetient.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task Register(AuthRegisterRequestDto authRegisterRequestDto);
        Task<AuthLoginResponseDto> Login(AuthLoginRequestDto authLoginRequestDto);
        Task<AuthRefreshResponseDto> Refresh(AuthRefreshRequestDto authRefreshRequestDto);
        Task Revoke();
    }
}

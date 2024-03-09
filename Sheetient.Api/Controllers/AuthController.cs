using Microsoft.AspNetCore.Mvc;
using Sheetient.App.Dtos.Auth;
using Sheetient.App.Services.Interfaces;

namespace Sheetient.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController(IAuthService authService) : BaseController
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequestDto authRegisterRequestDto)
        {
            await _authService.Register(authRegisterRequestDto);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType<AuthLoginResponseDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequestDto authLoginRequestDto)
        {
            var response = await _authService.Login(authLoginRequestDto);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType<AuthRefreshResponseDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] AuthRefreshRequestDto authRefreshRequestDto)
        {
            var response = await _authService.Refresh(authRefreshRequestDto);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Revoke()
        {
            await _authService.Revoke();
            return Ok();
        }
    }
}

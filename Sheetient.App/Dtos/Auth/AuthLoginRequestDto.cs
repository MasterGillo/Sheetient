namespace Sheetient.App.Dtos.Auth
{
    public class AuthLoginRequestDto
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
    }
}

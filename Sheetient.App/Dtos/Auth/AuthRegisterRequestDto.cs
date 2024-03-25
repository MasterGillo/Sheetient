namespace Sheetient.App.Dtos.Auth
{
    public class AuthRegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

namespace Sheetient.App.Dtos.Auth
{
    public class AuthRegisterRequestDto
    {
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

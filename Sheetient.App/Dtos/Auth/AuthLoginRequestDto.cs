namespace Sheetient.App.Dtos.Auth
{
    public record AuthLoginRequestDto
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

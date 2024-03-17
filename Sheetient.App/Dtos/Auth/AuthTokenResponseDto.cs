namespace Sheetient.App.Dtos.Auth
{
    public record AuthTokenResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public bool RememberMe { get; init; }
    }
}

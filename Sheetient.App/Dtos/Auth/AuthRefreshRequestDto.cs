namespace Sheetient.App.Dtos.Auth
{
    public class AuthRefreshRequestDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}

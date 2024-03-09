namespace Sheetient.App.Dtos.Auth
{
    public abstract class AuthTokenResponseBase
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}

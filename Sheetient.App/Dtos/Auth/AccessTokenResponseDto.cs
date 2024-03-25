namespace Sheetient.App.Dtos.Auth
{
    public record AccessTokenResponseDto
    {
        public AccessTokenResponseDto(string accessToken)
        {
            AccessToken = accessToken;
        }
        public string AccessToken { get; init; } = string.Empty;
    }
}

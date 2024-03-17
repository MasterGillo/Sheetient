namespace Sheetient.Domain.Interfaces
{
    public interface IJwtSettings
    {
        string Issuer { get; init; }
        string Audience { get; init; }
        string Key { get; init; }
        string AccessTokenName { get; init; }
        string RefreshTokenName { get; init; }
    }
}

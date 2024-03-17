using Sheetient.Domain.Interfaces;

namespace Sheetient.Infra.Settings
{
    public record JwtSettings : IJwtSettings
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required string Key { get; init; }
        public required string AccessTokenName { get; init; }
        public required string RefreshTokenName { get; init; }
    }
}

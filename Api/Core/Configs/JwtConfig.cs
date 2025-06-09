namespace Api.Core.Configs;

public class JwtConfig
{
    public required string Key { get; set; }

    public required string Issuer { get; set; }
}

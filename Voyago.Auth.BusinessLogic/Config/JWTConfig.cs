namespace Voyago.Auth.BusinessLogic.Config;
public class JWTConfig : IJWTConfig
{
    public string Audience { get; } = null!;
    public string Issuer { get; } = null!;
    public string HashKey { get; } = null!;
    private JWTConfig()
    {

    }
    public JWTConfig(string audience, string issuer, string hashKey)
    {
        Audience = audience;
        Issuer = issuer;
        HashKey = hashKey;
    }
}

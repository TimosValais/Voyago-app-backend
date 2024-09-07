namespace Voyago.Auth.BusinessLogic.Config;
public interface IJWTConfig
{
    public string Audience { get; }
    public string Issuer { get; }
    public string HashKey { get; }
}

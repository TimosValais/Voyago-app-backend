namespace Voyago.Auth.API.Constants;

internal static class ApiRoutes
{
    private const string ApiBase = "api";
    public static class AuthRoutes
    {
        private const string Base = $"{ApiBase}/auth";
        public const string Login = $"{Base}/login";
        public const string Register = $"{Base}/register";
    }
}

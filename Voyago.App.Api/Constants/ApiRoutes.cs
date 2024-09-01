namespace Voyago.App.Api.Constants;

internal static class ApiRoutes
{
    private const string ApiBase = "api";
    public static class UserProfileRoutes
    {
        private const string Base = $"{ApiBase}/userprofiles";
        public const string GetAll = Base;
        public const string Get = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Create = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }
}

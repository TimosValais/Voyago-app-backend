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
        public const string GetTasks = $"{Base}/{{id}}/tasks";
    }

    public static class TripRoutes
    {
        private const string Base = $"{ApiBase}/trips";
        public const string GetAll = Base;
        public const string Get = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Create = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetTasks = $"{Base}/{{id}}/tasks";
        public const string PostTask = $"{Base}/{{id}}/tasks";
        public const string UpdateTask = $"{Base}/{{id}}/tasks//{{taskId}}";
        public const string DeleteTask = $"{Base}/{{id}}/tasks//{{taskId}}";
    }
}

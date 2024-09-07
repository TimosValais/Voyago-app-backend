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
        public const string GetTrips = $"{Base}/{{id}}/trips";
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
        public const string UpdateTask = $"{Base}/{{id}}/tasks";
        public const string DeleteTask = $"{Base}/{{id}}/tasks/{{taskId}}";
        public const string PostUser = $"{Base}/{{id}}/users/{{userId}}";
        public const string UpdateUser = $"{Base}/{{id}}/users/{{userId}}";
        public const string DeleteUser = $"{Base}/{{id}}/users/{{userId}}";
    }

    public static class TaskRoutes
    {
        private const string Base = $"{ApiBase}/tasks";
        public const string AddUser = $"{Base}/{{id}}/users/{{userId}}";
        public const string RemoveUser = $"{Base}/{{id}}/users/{{userId}}";
    }
}

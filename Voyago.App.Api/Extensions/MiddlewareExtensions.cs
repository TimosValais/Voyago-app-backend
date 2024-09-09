using Voyago.App.Api.Constants;
using Voyago.App.Api.Middleware;

namespace Voyago.App.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseTaskRequestMiddleware(this IApplicationBuilder app)
    {
        app.UseWhen(
            context =>
            {
                string? requestPath = context.Request.Path.Value;
                bool isTargetRoute = requestPath != null
                    && requestPath.StartsWith($"/{ApiRoutes.TripRoutes.Base}", StringComparison.OrdinalIgnoreCase)
                    && requestPath.EndsWith("/tasks", StringComparison.OrdinalIgnoreCase);

                bool isPostOrPut = context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)
                    || context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase);

                return isTargetRoute && isPostOrPut;
            },
            appBuilder =>
            {
                appBuilder.UseMiddleware<TaskRequestMiddleware>();
            });

        return app;
    }
}

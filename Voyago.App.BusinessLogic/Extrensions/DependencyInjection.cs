using Microsoft.Extensions.DependencyInjection;
using Voyago.App.BusinessLogic.Services;

namespace Voyago.App.BusinessLogic.Extrensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ITripService, TripService>();
        services.AddTransient<ITripTaskService, TripTaskService>();
        services.AddTransient<IUserProfileService, UserProfileService>();
        return services;
    }
}

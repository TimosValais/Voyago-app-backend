using Microsoft.Extensions.DependencyInjection;
using Voyago.App.DataAccessLayer.Extensions;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.DataAccessLayer.Common;
public static class DependencyInjection
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlLiteConnectionFactory(connectionString));

        services.AddScoped<IGeneralBookingTaskRepository, GeneralBookingTaskRepository>();
        services.AddScoped<IFlightBookingTaskRepository, FlightBookingTaskRepository>();
        services.AddScoped<IHotelBookingTaskRepository, HotelBookingTaskRepository>();
        services.AddScoped<IOtherTaskRepository, OtherTaskRepository>();
        services.AddScoped<IPlanningTaskRepository, PlanningTaskRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ITripUserRoleRepository, TripUserRoleRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        return services;
    }
}

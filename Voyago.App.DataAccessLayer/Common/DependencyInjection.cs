using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Voyago.App.DataAccessLayer.Extensions;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.DataAccessLayer.Common;
public static class DependencyInjection
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, string connectionString)
    {
        //adding handler to make lists of string to json in the db
        SqlMapper.AddTypeHandler(new JsonListStringTypeHandler());
        services.AddSingleton<IDbConnectionFactory>(_ => new MySqlConnectionFactory(connectionString));
        services.AddSingleton<IDbInitializer, DbInitializer>();
        services.AddScoped<IGeneralBookingTaskRepository, GeneralBookingTaskRepository>();
        services.AddScoped<IFlightBookingTaskRepository, FlightBookingTaskRepository>();
        services.AddScoped<IHotelBookingTaskRepository, HotelBookingTaskRepository>();
        services.AddScoped<IOtherTaskRepository, OtherTaskRepository>();
        services.AddScoped<IPlanningTaskRepository, PlanningTaskRepository>();
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ITripUserRoleRepository, TripUserRoleRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ITaskUserRepository, TaskUserRepository>();
        return services;
    }
}

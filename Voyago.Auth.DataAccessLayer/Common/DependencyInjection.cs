using Microsoft.Extensions.DependencyInjection;
using Voyago.Auth.DataAccessLayer.Extensions;
using Voyago.Auth.DataAccessLayer.Repositories;

namespace Voyago.Auth.DataAccessLayer.Common;
public static class DependencyInjection
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbInitializer, DbInitializer>();
        services.AddSingleton<IDbConnectionFactory>(_ => new MySqlConnectionFactory(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}

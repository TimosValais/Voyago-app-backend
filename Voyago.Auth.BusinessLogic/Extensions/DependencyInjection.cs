using Microsoft.Extensions.DependencyInjection;
using Voyago.Auth.BusinessLogic.Services;

namespace Voyago.Auth.BusinessLogic.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}

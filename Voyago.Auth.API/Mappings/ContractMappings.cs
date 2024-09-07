using Voyago.App.Contracts.Requests;
using Voyago.Auth.DataAccessLayer.Entities;

namespace Voyago.Auth.API.Mappings;

internal static class ContractMappings
{
    public static User MapToUser(this RegisterUserRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.Password
        };
    }
}

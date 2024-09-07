using Voyago.Auth.DataAccessLayer.Entities;

namespace Voyago.Auth.BusinessLogic.Services;

public interface IAuthService
{
    Task<Guid> RegisterAsync(User user, CancellationToken cancellationToken = default);
    Task<string?> LoginAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
}

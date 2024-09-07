using Voyago.Auth.DataAccessLayer.Entities;

namespace Voyago.Auth.DataAccessLayer.Repositories;
public interface IUserRepository
{
    Task<bool> InsertAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string usernameOrEmail, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
}

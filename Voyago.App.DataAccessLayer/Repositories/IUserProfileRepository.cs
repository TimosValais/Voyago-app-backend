using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}

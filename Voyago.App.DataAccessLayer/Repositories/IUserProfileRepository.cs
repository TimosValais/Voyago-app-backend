using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationtoken = default);
    Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationtoken = default);
    Task<IEnumerable<UserProfile>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationtoken = default);
    Task<IEnumerable<UserProfile>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationtoken = default);
    Task<bool> InsertAsync(UserProfile userProfile, CancellationToken cancellationtoken = default);
    Task<bool> UpdateAsync(UserProfile userProfile, CancellationToken cancellationtoken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationtoken = default);

}

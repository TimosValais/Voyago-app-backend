using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.BusinessLogic.Services;
public interface IUserProfileService
{
    Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

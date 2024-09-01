using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface ITripUserRoleRepository
{
    Task<TripUserRoles?> GetByTripAndUserId(Guid tripId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TripUserRoles>> GetByTripId(Guid tripId, CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(TripUserRoles tripUserRoles, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TripUserRoles tripUserRoles, CancellationToken cancellationToken = default);
}

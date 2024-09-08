using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.BusinessLogic.Services;
public interface ITripUserRolesService
{
    Task<TripUserRoles?> GetTripUserRoles(Guid tripId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TripUserRoles>> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
}

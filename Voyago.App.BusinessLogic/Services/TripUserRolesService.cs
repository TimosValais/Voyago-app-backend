using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.BusinessLogic.Services;
public class TripUserRolesService : ITripUserRolesService
{
    private readonly ITripUserRoleRepository _tripUserRoleRepository;

    public TripUserRolesService(ITripUserRoleRepository tripUserRoleRepository)
    {
        _tripUserRoleRepository = tripUserRoleRepository;
    }

    public async Task<IEnumerable<TripUserRoles>> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _tripUserRoleRepository.GetByUserId(userId, cancellationToken);
    }


    public async Task<TripUserRoles?> GetTripUserRoles(Guid tripId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _tripUserRoleRepository.GetByTripAndUserId(tripId, userId);
    }
}

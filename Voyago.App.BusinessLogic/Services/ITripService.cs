using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.BusinessLogic.Services;
public interface ITripService
{
    Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpsertAsync(Trip trip, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellation = default);
    Task<bool> UpsertUser(Guid userId, Guid tripId, TripRole role, CancellationToken cancellationToken = default);
    Task<bool> RemoveUser(Guid userId, Guid tripId, CancellationToken cancellationToken = default);
}

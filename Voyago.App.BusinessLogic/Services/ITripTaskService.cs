using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.BusinessLogic.Services;
public interface ITripTaskService
{
    Task<IEnumerable<TripTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TripTask>> GetAllByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TripTask>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TripTask?> GetByIdAsync(Guid id, TaskType type, CancellationToken cancellationToken = default);
    Task<bool> UpsertAsync(TripTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, TaskType type, CancellationToken cancellationToken = default);

}

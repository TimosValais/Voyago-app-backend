using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IPlanningTaskRepository
{
    Task<PlanningTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanningTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(PlanningTask task, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(PlanningTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanningTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanningTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

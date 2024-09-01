using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IOtherTaskRepository
{
    Task<OtherTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(OtherTask task, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(OtherTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

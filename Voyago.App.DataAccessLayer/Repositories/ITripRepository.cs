using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(Trip trip, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Trip trip, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

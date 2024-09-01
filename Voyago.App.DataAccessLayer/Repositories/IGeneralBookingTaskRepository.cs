using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IGeneralBookingTaskRepository
{
    Task<GeneralBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GeneralBookingTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(GeneralBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(GeneralBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

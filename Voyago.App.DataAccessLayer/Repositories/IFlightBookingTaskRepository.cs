using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IFlightBookingTaskRepository
{
    Task<FlightBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightBookingTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(FlightBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(FlightBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightBookingTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightBookingTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

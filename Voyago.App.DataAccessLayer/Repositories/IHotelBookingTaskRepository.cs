using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface IHotelBookingTaskRepository
{
    Task<HotelBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<HotelBookingTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(HotelBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(HotelBookingTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<HotelBookingTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
    Task<IEnumerable<HotelBookingTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

}

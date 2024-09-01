using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Repositories;
public interface ITaskUserRepository
{
    Task<IEnumerable<UserProfile>> GetUsersFromTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TripTask>> GetTasksFromUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> InsertAsync(TaskUser taskUser, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TaskUser taskUser, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TaskUser taskUser, CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Logging;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Repositories;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.BusinessLogic.Services;
public class TripTaskService : ITripTaskService
{
    private readonly IFlightBookingTaskRepository _flightBookingTaskRepository;
    private readonly IGeneralBookingTaskRepository _generalBookingTaskRepository;
    private readonly IHotelBookingTaskRepository _hotelBookingTaskRepository;
    private readonly IOtherTaskRepository _otherTaskRepository;
    private readonly IPlanningTaskRepository _planningTaskRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITaskUserRepository _taskUserRepository;
    private readonly ILogger<TripTaskService> _logger;

    public TripTaskService(IFlightBookingTaskRepository flightBookingTaskRepository,
                           IGeneralBookingTaskRepository generalBookingTaskRepository,
                           IHotelBookingTaskRepository hotelBookingTaskRepository,
                           IOtherTaskRepository otherTaskRepository,
                           IPlanningTaskRepository planningTaskRepository,
                           IUserProfileRepository userProfileRepository,
                           ITaskUserRepository taskUserRepository,
                           ILogger<TripTaskService> logger)
    {
        _flightBookingTaskRepository = flightBookingTaskRepository;
        _generalBookingTaskRepository = generalBookingTaskRepository;
        _hotelBookingTaskRepository = hotelBookingTaskRepository;
        _otherTaskRepository = otherTaskRepository;
        _planningTaskRepository = planningTaskRepository;
        _userProfileRepository = userProfileRepository;
        _taskUserRepository = taskUserRepository;
        _logger = logger;


    }

    public async Task<bool> AddUser(Guid userId, Guid taskId, TaskType type, CancellationToken cancellationToken = default)
    {
        try
        {
            UserProfile? user = await _userProfileRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            return await _taskUserRepository.InsertAsync(new TaskUser() { TaskId = taskId, UserId = userId, });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, TaskType type, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (type)
            {
                case TaskType.GeneralBooking:
                    return await _generalBookingTaskRepository.DeleteAsync(id, cancellationToken);
                case TaskType.HotelBooking:
                    return await _hotelBookingTaskRepository.DeleteAsync(id, cancellationToken);
                case TaskType.TicketBooking:
                    return await _flightBookingTaskRepository.DeleteAsync(id, cancellationToken);
                case TaskType.Planning:
                    return await _planningTaskRepository.DeleteAsync(id, cancellationToken);
                case TaskType.Other:
                    return await _otherTaskRepository.DeleteAsync(id, cancellationToken);
                default:
                    throw new Exception("Wrong Task type");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

    }

    public async Task<IEnumerable<TripTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<TripTask> tasks = [];
        IEnumerable<FlightBookingTask> flightTasks = await _flightBookingTaskRepository.GetAllAsync(cancellationToken);
        IEnumerable<GeneralBookingTask> generalBookingTasks = await _generalBookingTaskRepository.GetAllAsync(cancellationToken);
        IEnumerable<HotelBookingTask> hotelBookingTasks = await _hotelBookingTaskRepository.GetAllAsync(cancellationToken);
        IEnumerable<OtherTask> otherTasks = await _otherTaskRepository.GetAllAsync(cancellationToken);
        IEnumerable<PlanningTask> planningTasks = await _planningTaskRepository.GetAllAsync(cancellationToken);
        tasks.AddRange(flightTasks);
        tasks.AddRange(generalBookingTasks);
        tasks.AddRange(hotelBookingTasks);
        tasks.AddRange(otherTasks);
        tasks.AddRange(planningTasks);
        return tasks;
    }

    public async Task<IEnumerable<TripTask>> GetAllByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        try
        {
            List<TripTask> tasks = [];
            IEnumerable<FlightBookingTask> flightTasks = await _flightBookingTaskRepository.GetByTripIdAsync(tripId, cancellationToken);
            IEnumerable<GeneralBookingTask> generalBookingTasks = await _generalBookingTaskRepository.GetByTripIdAsync(tripId, cancellationToken);
            IEnumerable<HotelBookingTask> hotelBookingTasks = await _hotelBookingTaskRepository.GetByTripIdAsync(tripId, cancellationToken);
            IEnumerable<OtherTask> otherTasks = await _otherTaskRepository.GetByTripIdAsync(tripId, cancellationToken);
            IEnumerable<PlanningTask> planningTasks = await _planningTaskRepository.GetByTripIdAsync(tripId, cancellationToken);
            tasks.AddRange(flightTasks);
            tasks.AddRange(generalBookingTasks);
            tasks.AddRange(hotelBookingTasks);
            tasks.AddRange(otherTasks);
            tasks.AddRange(planningTasks);
            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

    }

    public async Task<IEnumerable<TripTask>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            List<TripTask> tasks = [];
            IEnumerable<FlightBookingTask> flightTasks = await _flightBookingTaskRepository.GetByUserIdAsync(userId, cancellationToken);
            IEnumerable<GeneralBookingTask> generalBookingTasks = await _generalBookingTaskRepository.GetByUserIdAsync(userId, cancellationToken);
            IEnumerable<HotelBookingTask> hotelBookingTasks = await _hotelBookingTaskRepository.GetByUserIdAsync(userId, cancellationToken);
            IEnumerable<OtherTask> otherTasks = await _otherTaskRepository.GetByUserIdAsync(userId, cancellationToken);
            IEnumerable<PlanningTask> planningTasks = await _planningTaskRepository.GetByUserIdAsync(userId, cancellationToken);
            tasks.AddRange(flightTasks);
            tasks.AddRange(generalBookingTasks);
            tasks.AddRange(hotelBookingTasks);
            tasks.AddRange(otherTasks);
            tasks.AddRange(planningTasks);
            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<TripTask?> GetByIdAsync(Guid id, TaskType type, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (type)
            {
                case TaskType.GeneralBooking:
                    return await _generalBookingTaskRepository.GetByIdAsync(id, cancellationToken);
                case TaskType.HotelBooking:
                    return await _hotelBookingTaskRepository.GetByIdAsync(id, cancellationToken);
                case TaskType.TicketBooking:
                    return await _flightBookingTaskRepository.GetByIdAsync(id, cancellationToken);
                case TaskType.Planning:
                    return await _planningTaskRepository.GetByIdAsync(id, cancellationToken);
                case TaskType.Other:
                    return await _otherTaskRepository.GetByIdAsync(id, cancellationToken);
                default:
                    throw new Exception("Incorrect Task type");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> RemoveUser(Guid userId, Guid taskId, TaskType type, CancellationToken cancellationToken = default)
    {
        try
        {
            UserProfile? user = await _userProfileRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            return await _taskUserRepository.DeleteAsync(new TaskUser() { TaskId = taskId, UserId = userId, });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> UpsertAsync(TripTask task, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (task.Type)
            {
                case TaskType.GeneralBooking:
                    GeneralBookingTask? gbTask = task as GeneralBookingTask;
                    if (gbTask is null)
                    {
                        throw new Exception("Unexpected entity type");
                    }
                    GeneralBookingTask? existingGBEntity = await _generalBookingTaskRepository.GetByIdAsync(gbTask.Id, cancellationToken);
                    if (existingGBEntity is null)
                    {
                        return await _generalBookingTaskRepository.InsertAsync(gbTask, cancellationToken);
                    }
                    return await _generalBookingTaskRepository.UpdateAsync(gbTask, cancellationToken);
                case TaskType.HotelBooking:
                    HotelBookingTask? hTask = task as HotelBookingTask;
                    if (hTask is null)
                    {
                        throw new Exception("Unexpected entity type");
                    }
                    HotelBookingTask? existingEntity = await _hotelBookingTaskRepository.GetByIdAsync(hTask.Id, cancellationToken);
                    if (existingEntity is null)
                    {
                        return await _hotelBookingTaskRepository.InsertAsync(hTask, cancellationToken);
                    }
                    return await _hotelBookingTaskRepository.UpdateAsync(hTask, cancellationToken);
                case TaskType.TicketBooking:
                    FlightBookingTask? fbTask = task as FlightBookingTask;
                    if (fbTask is null)
                    {
                        throw new Exception("Unexpected entity type");
                    }
                    GeneralBookingTask? existingFlightEntity = await _generalBookingTaskRepository.GetByIdAsync(fbTask.Id, cancellationToken);
                    if (existingFlightEntity is null)
                    {
                        return await _flightBookingTaskRepository.InsertAsync(fbTask, cancellationToken);
                    }
                    return await _flightBookingTaskRepository.UpdateAsync(fbTask, cancellationToken);
                case TaskType.Planning:
                    PlanningTask? pTask = task as PlanningTask;
                    if (pTask is null)
                    {
                        throw new Exception("Unexpected entity type");
                    }
                    PlanningTask? existingPlanningEntity = await _planningTaskRepository.GetByIdAsync(pTask.Id, cancellationToken);
                    if (existingPlanningEntity is null)
                    {
                        return await _planningTaskRepository.InsertAsync(pTask, cancellationToken);
                    }
                    return await _planningTaskRepository.UpdateAsync(pTask, cancellationToken);
                case TaskType.Other:
                    OtherTask? oTask = task as OtherTask;
                    if (oTask is null)
                    {
                        throw new Exception("Unexpected entity type");
                    }
                    OtherTask? existingOtherEntity = await _otherTaskRepository.GetByIdAsync(oTask.Id, cancellationToken);
                    if (existingOtherEntity is null)
                    {
                        return await _otherTaskRepository.InsertAsync(oTask, cancellationToken);
                    }
                    return await _otherTaskRepository.UpdateAsync(oTask, cancellationToken);
                default:
                    throw new Exception("Incorrect Task Type");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

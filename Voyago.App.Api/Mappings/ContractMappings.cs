using Voyago.App.Contracts.Requests;
using Voyago.App.Contracts.Responses;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.ValueObjects;
using ContractValueObjects = Voyago.App.Contracts.ValueObjects;
using EntityValueObjects = Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.Api.Mappings;

public static class ContractMappings
{
    public static UserProfileResponse MapToResponse(this UserProfile userProfile)
    {
        return new(
                Id: userProfile.Id,
                Email: userProfile.Email,
                Name: userProfile.Name,
                ProfilePictureUrl: userProfile.ProfilePictureUrl
            );
    }

    public static IEnumerable<UserProfileResponse> MapToResponses(this IEnumerable<UserProfile> userProfiles)
    {
        foreach (UserProfile profile in userProfiles)
        {
            yield return profile.MapToResponse();
        }
    }

    public static TripResponse MapToResponse(this Trip entity, EntityValueObjects.TripRole role)
    {
        return new TripResponse(
            Id: entity.Id,
            TripStatus: entity.TripStatus.MapToResponseTripStatus(),
            Budget: entity.Budget,
            From: entity.From,
            To: entity.To,
            IsOverBudget: entity.IsOverBudget,
            Role: role.MapToContractTripRole(),
            Users: entity.TripUsers.MapToResponses(),
            FlightTasks: entity.Tasks.MapToFlightTaskResponses(),
            GeneralTasks: entity.Tasks.MapToGeneralTaskResponses(),
            HotelTasks: entity.Tasks.MapToHotelTaskResponses(),
            OtherTasks: entity.Tasks.MapToOtherTaskResponses(),
            PlanningTasks: entity.Tasks.MapToPlanningTaskResponses()
            );
    }
    public static IEnumerable<TripResponse> MapToResponses(this IEnumerable<Trip> entities, IEnumerable<TripUserRoles> tripUserRoles)
    {
        foreach (Trip entity in entities)
        {
            TripUserRoles? tripUserRole = tripUserRoles.FirstOrDefault(tur => tur.TripId == entity.Id);
            EntityValueObjects.TripRole role = tripUserRole?.Role ?? EntityValueObjects.TripRole.Member;
            yield return entity.MapToResponse(role);
        }
    }

    public static FlightTaskResponse MapToResponse(this FlightBookingTask task)
    {
        return new FlightTaskResponse(
                task.Id,
                task.DepartureDate,
                task.ReturnDate,
                task.TripId,
                task.Users.MapToResponses(),
                task.Status.MapToContractStatus(),
                task.Deadline,
                task.Description,
                task.Name,
                task.MoneySpent,
                task.DocumentsUrls
            );
    }

    public static GeneralTaskResponse MapToResponse(this GeneralBookingTask task)
    {
        return new GeneralTaskResponse(
                task.Id,
                task.Notes,
                task.TripId,
                task.Users.MapToResponses(),
                task.Status.MapToContractStatus(),
                task.Deadline,
                task.Description,
                task.Name,
                task.MoneySpent,
                task.DocumentsUrls
            );
    }

    public static HotelTaskResponse MapToResponse(this HotelBookingTask task)
    {
        return new HotelTaskResponse(
                task.Id,
                task.CheckInDate,
                task.CheckOutDate,
                task.ContactNo,
                task.TripId,
                task.Users.MapToResponses(),
                task.Status.MapToContractStatus(),
                task.Deadline,
                task.Description,
                task.Name,
                task.MoneySpent,
                task.DocumentsUrls
            );
    }

    public static OtherTaskResponse MapToResponse(this OtherTask task)
    {
        return new OtherTaskResponse(
                task.Id,
                task.TripId,
                task.Users.MapToResponses(),
                task.Status.MapToContractStatus(),
                task.Deadline,
                task.Description,
                task.Name,
                task.MoneySpent,
                task.DocumentsUrls
            );
    }

    public static PlanningTaskResponse MapToResponse(this PlanningTask task)
    {
        return new PlanningTaskResponse(
                task.Id,
                task.Steps,
                task.TripId,
                task.Users.MapToResponses(),
                task.Status.MapToContractStatus(),
                task.Deadline,
                task.Description,
                task.Name,
                task.MoneySpent,
                task.DocumentsUrls
            );
    }

    public static IEnumerable<FlightTaskResponse> MapToResponses(IEnumerable<FlightBookingTask> tasks)
    {
        foreach (FlightBookingTask task in tasks)
        {
            yield return task.MapToResponse();
        }
    }
    public static IEnumerable<GeneralTaskResponse> MapToResponses(IEnumerable<GeneralBookingTask> tasks)
    {
        foreach (GeneralBookingTask task in tasks)
        {
            yield return task.MapToResponse();
        }
    }
    public static IEnumerable<HotelTaskResponse> MapToResponses(IEnumerable<HotelBookingTask> tasks)
    {
        foreach (HotelBookingTask task in tasks)
        {
            yield return task.MapToResponse();
        }
    }
    public static IEnumerable<OtherTaskResponse> MapToResponses(IEnumerable<OtherTask> tasks)
    {
        foreach (OtherTask task in tasks)
        {
            yield return task.MapToResponse();
        }
    }
    public static IEnumerable<PlanningTaskResponse> MapToResponses(IEnumerable<PlanningTask> tasks)
    {
        foreach (PlanningTask task in tasks)
        {
            yield return task.MapToResponse();
        }
    }
    public static Trip MapToEntity(this CreateTripRequest request, Guid id)
    {
        return new()
        {
            Id = id,
            Budget = request.Budget,
            From = request.From,
            To = request.To,
            TripStatus = TripStatus.Pending
        };
    }
    public static Trip MapToEntity(this UpdateTripRequest request, Guid id)
    {
        return new()
        {
            Id = id,
            Budget = request.Budget,
            From = request.From,
            To = request.To,
            TripStatus = request.TripStatus.MapToEntityStatus()
        };
    }

    public static TripTask MapCreateRequestToEntity(this ITaskRequest request, Guid tripId, Guid taskId)
    {
        TripTask taskToReturn = new();
        switch (request.TaskType)
        {
            case ContractValueObjects.TaskType.GeneralBooking:
                taskToReturn = (request as CreateGeneralTaskRequest)!.MapToEntity(tripId, taskId);
                break;
            case ContractValueObjects.TaskType.HotelBooking:
                taskToReturn = (request as CreateHotelTaskRequest)!.MapToEntity(tripId, taskId);
                break;
            case ContractValueObjects.TaskType.TicketBooking:
                taskToReturn = (request as CreateFlightTaskRequest)!.MapToEntity(tripId, taskId);
                break;
            case ContractValueObjects.TaskType.Planning:
                taskToReturn = (request as CreatePlanningTaskRequest)!.MapToEntity(tripId, taskId);
                break;
            case ContractValueObjects.TaskType.Other:
                taskToReturn = (request as CreateOtherTaskRequest)!.MapToEntity(tripId, taskId);
                break;
        }
        return taskToReturn;
    }

    public static TripTask MapUpdateRequestToEntity(this ITaskRequest request, Guid tripId)
    {
        TripTask taskToReturn = new();
        switch (request.TaskType)
        {
            case ContractValueObjects.TaskType.GeneralBooking:
                taskToReturn = (request as UpdateGeneralTaskRequest)!.MapToEntity(tripId);
                break;
            case ContractValueObjects.TaskType.HotelBooking:
                taskToReturn = (request as UpdateHotelTaskRequest)!.MapToEntity(tripId);
                break;
            case ContractValueObjects.TaskType.TicketBooking:
                taskToReturn = (request as UpdateFlightTaskRequest)!.MapToEntity(tripId);
                break;
            case ContractValueObjects.TaskType.Planning:
                taskToReturn = (request as UpdatePlanningTaskRequest)!.MapToEntity(tripId);
                break;
            case ContractValueObjects.TaskType.Other:
                taskToReturn = (request as UpdateOtherTaskRequest)!.MapToEntity(tripId);
                break;
        }
        return taskToReturn;
    }
    public static EntityValueObjects.TaskType MapToEntity(this ContractValueObjects.TaskType type)
    {
        switch (type)
        {
            case ContractValueObjects.TaskType.GeneralBooking:
                return EntityValueObjects.TaskType.GeneralBooking;
            case ContractValueObjects.TaskType.HotelBooking:
                return EntityValueObjects.TaskType.GeneralBooking;
            case ContractValueObjects.TaskType.TicketBooking:
                return EntityValueObjects.TaskType.GeneralBooking;
            case ContractValueObjects.TaskType.Planning:
                return EntityValueObjects.TaskType.GeneralBooking;
            case ContractValueObjects.TaskType.Other:
                return EntityValueObjects.TaskType.GeneralBooking;
            default:
                throw new NotImplementedException();
        }
    }
    public static EntityValueObjects.TripRole MapToEntityTripRole(this ContractValueObjects.TripRole role)
    {
        switch (role)
        {
            case ContractValueObjects.TripRole.Member:
                return EntityValueObjects.TripRole.Member;
            case ContractValueObjects.TripRole.Manager:
                return EntityValueObjects.TripRole.Manager;
            case ContractValueObjects.TripRole.Admin:
                return EntityValueObjects.TripRole.Admin;
            default:
                return EntityValueObjects.TripRole.Member;
        }
    }

    public static UserProfile MapToEntity(this UpdateUserProfileRequest request, Guid id)
    {
        return new()
        {
            Email = request.Email,
            Name = request.Name,
            Id = id
        };
    }

    private static GeneralBookingTask MapToEntity(this CreateGeneralTaskRequest request, Guid tripId, Guid taskId)
    {
        return new()
        {
            Deadline = request.DeadLine,
            Description = request.Description,
            Id = taskId,
            MoneySpent = request.MoneySpent,
            Name = request.Name,
            Notes = request.Notes,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.GeneralBooking
        };
    }
    private static FlightBookingTask MapToEntity(this CreateFlightTaskRequest request, Guid tripId, Guid taskId)
    {
        return new()
        {
            Id = taskId,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            DepartureDate = request.DepartureDate,
            MoneySpent = request.MoneySpent,
            ReturnDate = request.ReturnDate,
            Status = StatusTask.Pending,
            TripId = tripId,
            Type = TaskType.TicketBooking
        };
    }
    private static HotelBookingTask MapToEntity(this CreateHotelTaskRequest request, Guid tripId, Guid taskId)
    {
        return new()
        {
            Id = taskId,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            ContactNo = request.ContactNo,
            MoneySpent = request.MoneySpent,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.HotelBooking
        };
    }
    private static OtherTask MapToEntity(this CreateOtherTaskRequest request, Guid tripId, Guid taskId)
    {
        return new()
        {
            Id = taskId,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            MoneySpent = request.MoneySpent,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.Other
        };
    }
    private static PlanningTask MapToEntity(this CreatePlanningTaskRequest request, Guid tripId, Guid taskId)
    {
        return new()
        {
            Id = taskId,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            MoneySpent = request.MoneySpent,
            Steps = request.Steps,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.Planning

        };
    }
    private static GeneralBookingTask MapToEntity(this UpdateGeneralTaskRequest request, Guid tripId)
    {
        return new()
        {
            Deadline = request.DeadLine,
            Description = request.Description,
            Id = request.Id,
            MoneySpent = request.MoneySpent,
            Name = request.Name,
            Notes = request.Notes,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.GeneralBooking
        };
    }
    private static FlightBookingTask MapToEntity(this UpdateFlightTaskRequest request, Guid tripId)
    {
        return new()
        {
            Id = request.Id,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            DepartureDate = request.DepartureDate,
            MoneySpent = request.MoneySpent,
            ReturnDate = request.ReturnDate,
            Status = StatusTask.Pending,
            TripId = tripId,
            Type = TaskType.TicketBooking
        };
    }
    private static HotelBookingTask MapToEntity(this UpdateHotelTaskRequest request, Guid tripId)
    {
        return new()
        {
            Id = request.Id,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            ContactNo = request.ContactNo,
            MoneySpent = request.MoneySpent,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.HotelBooking
        };
    }
    private static OtherTask MapToEntity(this UpdateOtherTaskRequest request, Guid tripId)
    {
        return new()
        {
            Id = request.Id,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            MoneySpent = request.MoneySpent,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.Other
        };
    }
    private static PlanningTask MapToEntity(this UpdatePlanningTaskRequest request, Guid tripId)
    {
        return new()
        {
            Id = request.Id,
            Deadline = request.DeadLine,
            Description = request.Description,
            Name = request.Name,
            MoneySpent = request.MoneySpent,
            Steps = request.Steps,
            TripId = tripId,
            Status = StatusTask.Pending,
            Type = TaskType.Planning

        };
    }
    private static IEnumerable<FlightTaskResponse> MapToFlightTaskResponses(this IEnumerable<TripTask> tasks)
    {
        IEnumerable<TripTask> flightTasks = tasks.Where(t => t.Type == EntityValueObjects.TaskType.TicketBooking);
        foreach (TripTask task in flightTasks)
        {
            if (task is not FlightBookingTask || task is null)
            {
                continue;
            }
            yield return (task as FlightBookingTask)!.MapToResponse();
        }
    }
    private static IEnumerable<GeneralTaskResponse> MapToGeneralTaskResponses(this IEnumerable<TripTask> tasks)
    {
        IEnumerable<TripTask> generalTask = tasks.Where(t => t.Type == EntityValueObjects.TaskType.GeneralBooking);
        foreach (TripTask task in generalTask)
        {
            if (task is not GeneralBookingTask || task is null)
            {
                continue;
            }
            yield return (task as GeneralBookingTask)!.MapToResponse();
        }
    }
    private static IEnumerable<HotelTaskResponse> MapToHotelTaskResponses(this IEnumerable<TripTask> tasks)
    {
        IEnumerable<TripTask> hotelTasks = tasks.Where(t => t.Type == EntityValueObjects.TaskType.HotelBooking);
        foreach (TripTask task in hotelTasks)
        {
            if (task is not HotelBookingTask || task is null)
            {
                continue;
            }
            yield return (task as HotelBookingTask)!.MapToResponse();
        }
    }
    private static IEnumerable<OtherTaskResponse> MapToOtherTaskResponses(this IEnumerable<TripTask> tasks)
    {
        IEnumerable<TripTask> otherTasks = tasks.Where(t => t.Type == EntityValueObjects.TaskType.Other);
        foreach (TripTask task in otherTasks)
        {
            if (task is not OtherTask || task is null)
            {
                continue;
            }
            yield return (task as OtherTask)!.MapToResponse();
        }
    }
    private static IEnumerable<PlanningTaskResponse> MapToPlanningTaskResponses(this IEnumerable<TripTask> tasks)
    {
        IEnumerable<TripTask> planningTasks = tasks.Where(t => t.Type == EntityValueObjects.TaskType.Planning);
        foreach (TripTask task in planningTasks)
        {
            if (task is not PlanningTask || task is null)
            {
                continue;
            }
            yield return (task as PlanningTask)!.MapToResponse();
        }
    }
    private static ContractValueObjects.StatusTask MapToContractStatus(this EntityValueObjects.StatusTask status)
    {
        switch (status)
        {
            case EntityValueObjects.StatusTask.Canceled:
                return ContractValueObjects.StatusTask.Canceled;
            case EntityValueObjects.StatusTask.Pending:
                return ContractValueObjects.StatusTask.Pending;
            case EntityValueObjects.StatusTask.Started:
                return ContractValueObjects.StatusTask.Started;
            case EntityValueObjects.StatusTask.Completed:
                return ContractValueObjects.StatusTask.Completed;
            default:
                return ContractValueObjects.StatusTask.Pending;
        }
    }
    private static EntityValueObjects.TripStatus MapToEntityStatus(this ContractValueObjects.TripStatus status)
    {
        switch (status)
        {
            case ContractValueObjects.TripStatus.Pending:
                return EntityValueObjects.TripStatus.Pending;
            case ContractValueObjects.TripStatus.OnGoing:
                return EntityValueObjects.TripStatus.OnGoing;
            case ContractValueObjects.TripStatus.Completed:
                return EntityValueObjects.TripStatus.Completed;
            default:
                return EntityValueObjects.TripStatus.Pending;
        }
    }
    private static UserProfileResponse MapToResponse(this TripUserRoles userRole)
    {
        return userRole.User.MapToResponse();
    }
    private static IEnumerable<UserProfileResponse> MapToResponses(this IEnumerable<TripUserRoles> userRoles)
    {
        foreach (TripUserRoles userRole in userRoles)
        {
            yield return userRole.MapToResponse();
        }
    }

    private static ContractValueObjects.TripRole MapToContractTripRole(this EntityValueObjects.TripRole role)
    {
        switch (role)
        {
            case EntityValueObjects.TripRole.Member:
                return ContractValueObjects.TripRole.Member;
            case EntityValueObjects.TripRole.Manager:
                return ContractValueObjects.TripRole.Manager;
            case EntityValueObjects.TripRole.Admin:
                return ContractValueObjects.TripRole.Admin;
            default:
                return ContractValueObjects.TripRole.Member;
        }
    }
    private static ContractValueObjects.TripStatus MapToResponseTripStatus(this EntityValueObjects.TripStatus status)
    {
        switch (status)
        {
            case EntityValueObjects.TripStatus.Pending:
                return ContractValueObjects.TripStatus.Pending;
            case EntityValueObjects.TripStatus.OnGoing:
                return ContractValueObjects.TripStatus.OnGoing;
            case EntityValueObjects.TripStatus.Completed:
                return ContractValueObjects.TripStatus.Completed;
            default:
                return ContractValueObjects.TripStatus.Pending;
        }
    }

}

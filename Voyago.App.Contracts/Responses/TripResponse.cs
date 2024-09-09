using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Responses;
public record TripResponse
    (
        Guid Id, TripStatus TripStatus,
        string Name,
        decimal Budget,
        DateTime From,
        DateTime To,
        bool IsOverBudget,
        TripRole Role,
        IEnumerable<UserProfileResponse> Users,
        IEnumerable<FlightTaskResponse> FlightTasks,
        IEnumerable<GeneralTaskResponse> GeneralTasks,
        IEnumerable<HotelTaskResponse> HotelTasks,
        IEnumerable<OtherTaskResponse> OtherTasks,
        IEnumerable<PlanningTaskResponse> PlanningTasks
    );

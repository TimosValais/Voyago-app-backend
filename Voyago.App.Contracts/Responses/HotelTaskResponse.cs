using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Responses;
public record HotelTaskResponse
    (
        Guid Id,
        DateTime? CheckInDate,
        DateTime? CheckOutDate,
        string? ContactNo,
        Guid TripId,
        IEnumerable<UserProfileResponse> Users,
        StatusTask Status,
        DateTime DeadLine,
        string? Description,
        string Name,
        decimal MoneySpent,
        IEnumerable<string> DocumentsUrls,
        TaskType Type = TaskType.HotelBooking
    );

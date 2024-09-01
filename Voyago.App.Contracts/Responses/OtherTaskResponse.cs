using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Responses;
public record OtherTaskResponse
    (
        Guid Id,
        Guid TripId,
        IEnumerable<UserProfileResponse> Users,
        StatusTask Status,
        DateTime DeadLine,
        string? Description,
        string Name,
        decimal MoneySpent,
        IEnumerable<string> DocumentsUrls,
        TaskType Type = TaskType.Other
    );

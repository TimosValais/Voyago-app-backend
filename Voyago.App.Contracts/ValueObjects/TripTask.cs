using Voyago.App.Contracts.Responses;

namespace Voyago.App.Contracts.ValueObjects;
public record TripTask
    (
        Guid TripId,
        TaskType Type,
        IEnumerable<UserProfileResponse> Users,
        StatusTask Status,
        DateTime DeadLine,
        string? Description,
        string Name,
        decimal MoneySpent,
        IEnumerable<string> DocumentsUrls
    );

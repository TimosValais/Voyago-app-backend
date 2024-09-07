using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public record UpdateGeneralTaskRequest(
        Guid Id,
        IEnumerable<string> Notes,
        DateTime DeadLine,
        string? Description,
        string Name,
        decimal MoneySpent,
        IEnumerable<byte[]> Documents) : ITaskRequest
{
    public TaskType TaskType => TaskType.GeneralBooking;
};
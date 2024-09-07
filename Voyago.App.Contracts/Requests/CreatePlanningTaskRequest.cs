using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public record CreatePlanningTaskRequest(
    IEnumerable<string> Steps,
    DateTime DeadLine,
    string? Description,
    string Name,
    decimal MoneySpent,
    IEnumerable<byte[]> Documents) : ITaskRequest
{
    public TaskType TaskType => TaskType.Planning;
};

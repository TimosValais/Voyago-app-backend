using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public class UpdatePlanningTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.Planning;
    public Guid Id { get; set; }
    public IEnumerable<string> Steps { get; set; } = [];
}
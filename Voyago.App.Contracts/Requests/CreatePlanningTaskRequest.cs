using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public class CreatePlanningTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.Planning;
    public IEnumerable<string> Steps { get; set; } = [];
}
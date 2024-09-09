using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public class UpdateOtherTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.Other;

    public Guid Id { get; set; }
}

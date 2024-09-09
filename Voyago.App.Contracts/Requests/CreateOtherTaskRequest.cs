using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public class CreateOtherTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.Other;

}
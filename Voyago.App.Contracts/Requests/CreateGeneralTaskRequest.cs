using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public class CreateGeneralTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.GeneralBooking;

    public IEnumerable<string> Notes { get; set; } = [];
}
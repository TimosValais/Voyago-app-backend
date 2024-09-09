using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public class UpdateGeneralTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.GeneralBooking;

    public Guid Id { get; set; }
    public IEnumerable<string> Notes { get; set; } = [];
}
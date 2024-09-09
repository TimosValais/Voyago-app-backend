using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public class CreateHotelTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.HotelBooking;

    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? ContactNo { get; set; }
}

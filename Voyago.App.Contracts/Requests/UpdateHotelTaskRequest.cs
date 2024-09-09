using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public class UpdateHotelTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.HotelBooking;
    public Guid Id { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public string? ContactNo { get; set; }
}
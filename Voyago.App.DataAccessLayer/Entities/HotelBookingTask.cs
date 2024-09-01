using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class HotelBookingTask : TripTask
{
    public Guid Id { get; set; }
    public override TaskType Type => TaskType.HotelBooking;
    public DateTime? CheckInDate { get; set; } = null;
    public DateTime? CheckOutDate { get; set; } = null;
    public string? ContactNo { get; set; } = null;
}

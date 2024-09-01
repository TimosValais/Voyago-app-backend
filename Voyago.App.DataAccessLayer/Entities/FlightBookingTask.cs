using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class FlightBookingTask : TripTask
{
    public Guid Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public override TaskType Type => TaskType.TicketBooking;
}

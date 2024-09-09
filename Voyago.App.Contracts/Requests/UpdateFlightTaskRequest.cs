using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public class UpdateFlightTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.TicketBooking;
    public Guid Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public class CreateFlightTaskRequest : BaseTaskRequest
{
    public override TaskType TaskType => TaskType.TicketBooking;
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }

}

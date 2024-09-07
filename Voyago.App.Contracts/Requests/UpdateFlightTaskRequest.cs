using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;
public record UpdateFlightTaskRequest(
    Guid Id,
    DateTime DepartureDate,
    DateTime ReturnDate,
    DateTime DeadLine,
    string? Description,
    string Name,
    decimal MoneySpent,
    IEnumerable<byte[]> Documents) : ITaskRequest
{
    public TaskType TaskType => TaskType.TicketBooking;
};
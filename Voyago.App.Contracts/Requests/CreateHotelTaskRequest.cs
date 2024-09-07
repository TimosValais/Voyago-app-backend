using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public record CreateHotelTaskRequest(
    DateTime? CheckInDate,
    DateTime? CheckOutDate,
    string? ContactNo,
    DateTime DeadLine,
    string? Description,
    string Name,
    decimal MoneySpent,
    IEnumerable<byte[]> Documents) : ITaskRequest
{
    public TaskType TaskType => TaskType.HotelBooking;
};

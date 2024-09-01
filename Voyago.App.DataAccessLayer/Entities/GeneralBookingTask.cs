using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class GeneralBookingTask : TripTask
{
    public Guid Id { get; set; }
    public override TaskType Type => TaskType.GeneralBooking;
    public IEnumerable<string> Notes { get; set; } = [];
}

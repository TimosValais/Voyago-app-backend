using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class PlanningTask : TripTask
{
    public Guid Id { get; set; }
    public override TaskType Type => TaskType.Planning;
    public IEnumerable<string> Steps { get; set; } = [];

}

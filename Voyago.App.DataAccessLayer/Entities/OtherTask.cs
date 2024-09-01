using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class OtherTask : TripTask
{
    public Guid Id { get; set; }
    public override TaskType Type => TaskType.Other;

}

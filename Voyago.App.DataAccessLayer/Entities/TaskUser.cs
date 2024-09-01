using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class TaskUser
{
    public Guid UserId { get; set; }
    public virtual UserProfile? User { get; set; }
    public Guid TaskId { get; set; }
    public virtual TripTask? Task { get; set; }
}

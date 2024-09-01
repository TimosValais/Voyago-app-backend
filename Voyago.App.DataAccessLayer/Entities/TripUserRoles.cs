using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class TripUserRoles
{
    public Guid UserId { get; set; }
    public virtual UserProfile User { get; set; } = null!;
    public Guid TripId { get; set; }
    public virtual Trip Trip { get; set; } = null!;
    public TripRole Role { get; set; } = TripRole.Member;
}

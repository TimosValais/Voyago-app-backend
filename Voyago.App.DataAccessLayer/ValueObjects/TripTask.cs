using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.DataAccessLayer.ValueObjects;
public class TripTask
{
    public Guid TripId { get; set; }
    public virtual TaskType Type { get; set; }
    public List<UserProfile> Users { get; set; } = [];
    public StatusTask Status { get; set; }
    public DateTime Deadline { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public decimal MoneySpent { get; set; }
    public IEnumerable<string> DocumentsUrls { get; set; } = [];

}

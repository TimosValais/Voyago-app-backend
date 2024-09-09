namespace Voyago.App.Contracts.ValueObjects;
public class BaseTaskRequest
{
    public virtual TaskType TaskType { get; set; }
    public DateTime DeadLine { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public decimal MoneySpent { get; set; }
}

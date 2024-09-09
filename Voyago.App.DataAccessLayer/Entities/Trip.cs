using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class Trip
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public TripStatus TripStatus { get; set; } = TripStatus.Pending;
    public virtual List<TripUserRoles> TripUsers { get; set; } = [];
    public virtual List<TripTask> Tasks { get; set; } = [];
    public decimal Budget { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }
    public bool IsOverBudget => CheckOverBudget();

    public bool CheckOverBudget()
    {
        decimal moneySpent = 0;
        foreach (TripTask task in Tasks)
        {
            moneySpent += task.MoneySpent;
        }
        return moneySpent >= Budget;
    }
}

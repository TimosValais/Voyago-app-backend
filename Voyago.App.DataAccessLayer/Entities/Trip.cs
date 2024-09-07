using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Entities;
public class Trip
{
    public Guid Id { get; set; }
    public TripStatus TripStatus { get; set; } = TripStatus.Pending;
    public virtual IEnumerable<TripUserRoles> TripUsers { get; set; } = [];
    public virtual IEnumerable<TripTask> Tasks { get; set; } = [];
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

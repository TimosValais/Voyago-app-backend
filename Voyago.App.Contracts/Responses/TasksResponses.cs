namespace Voyago.App.Contracts.Responses;
public class TasksResponses
{
    public IEnumerable<FlightTaskResponse> FlightTasks { get; set; } = [];
    public IEnumerable<GeneralTaskResponse> GeneralTasks { get; set; } = [];
    public IEnumerable<HotelTaskResponse> HotelTasks { get; set; } = [];
    public IEnumerable<OtherTaskResponse> OtherTasks { get; set; } = [];
    public IEnumerable<PlanningTaskResponse> PlanningTasks { get; set; } = [];
}

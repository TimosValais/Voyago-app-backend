using Voyago.App.Contracts.Requests;

namespace Voyago.App.Api.Helpers;

internal static class TaskHelpers
{
    public static IEnumerable<byte[]>? GetCreateTaskDocuments(ITaskRequest request)
    {
        switch (request.TaskType)
        {
            case Contracts.ValueObjects.TaskType.GeneralBooking:
                return (request as CreateGeneralTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.HotelBooking:
                return (request as CreateHotelTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.TicketBooking:
                return (request as CreateFlightTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.Planning:
                return (request as CreatePlanningTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.Other:
                return (request as CreateOtherTaskRequest)!.Documents;
            default:
                return null;
        }
    }
    public static IEnumerable<byte[]>? GetUpdateTaskDocuments(ITaskRequest request)
    {
        switch (request.TaskType)
        {
            case Contracts.ValueObjects.TaskType.GeneralBooking:
                return (request as UpdateGeneralTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.HotelBooking:
                return (request as UpdateHotelTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.TicketBooking:
                return (request as UpdateFlightTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.Planning:
                return (request as UpdatePlanningTaskRequest)!.Documents;
            case Contracts.ValueObjects.TaskType.Other:
                return (request as UpdateOtherTaskRequest)!.Documents;
            default:
                return null;
        }
    }
}

using EntityValueObjects = Voyago.App.DataAccessLayer.ValueObjects;
using MessageValueObjects = Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.BusinessLogic.Mappings;
internal static class MessageMappings
{
    public static EntityValueObjects.TaskType MapToEntity(this MessageValueObjects.TaskType type)
    {
        switch (type)
        {
            case MessageValueObjects.TaskType.GeneralBooking:
                return EntityValueObjects.TaskType.GeneralBooking;
            case MessageValueObjects.TaskType.HotelBooking:
                return EntityValueObjects.TaskType.HotelBooking;
            case MessageValueObjects.TaskType.TicketBooking:
                return EntityValueObjects.TaskType.TicketBooking;
            case MessageValueObjects.TaskType.Planning:
                return EntityValueObjects.TaskType.Planning;
            case MessageValueObjects.TaskType.Other:
                return EntityValueObjects.TaskType.Other;
            default:
                return EntityValueObjects.TaskType.Other;
        }
    }
}

using Newtonsoft.Json;
using Voyago.App.Contracts.Requests;
using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Api.Middleware;

public class TaskRequestMiddleware
{
    private readonly RequestDelegate _next;

    public TaskRequestMiddleware(RequestDelegate next)
    {
        _next = next;

    }

    public async Task InvokeAsync(HttpContext context)
    {

        // Read the request body as a string
        context.Request.EnableBuffering();
        string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        // Attempt to deserialize to BaseTaskRequest
        BaseTaskRequest? baseRequest = JsonConvert.DeserializeObject<BaseTaskRequest>(requestBody);

        // Attempt to determine the specific task type
        if (baseRequest != null && Enum.TryParse(baseRequest.TaskType.ToString(), out TaskType taskType))
        {
            BaseTaskRequest? derivedRequest = null;
            if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                // Deserialize to the correct derived type based on TaskType
                derivedRequest = taskType switch
                {
                    TaskType.GeneralBooking => JsonConvert.DeserializeObject<CreateGeneralTaskRequest>(requestBody),
                    TaskType.HotelBooking => JsonConvert.DeserializeObject<CreateHotelTaskRequest>(requestBody),
                    TaskType.TicketBooking => JsonConvert.DeserializeObject<CreateFlightTaskRequest>(requestBody),
                    TaskType.Planning => JsonConvert.DeserializeObject<CreatePlanningTaskRequest>(requestBody),
                    TaskType.Other => JsonConvert.DeserializeObject<CreateOtherTaskRequest>(requestBody),
                    _ => null
                };

            }
            else if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                derivedRequest = taskType switch
                {
                    TaskType.GeneralBooking => JsonConvert.DeserializeObject<UpdateGeneralTaskRequest>(requestBody),
                    TaskType.HotelBooking => JsonConvert.DeserializeObject<UpdateHotelTaskRequest>(requestBody),
                    TaskType.TicketBooking => JsonConvert.DeserializeObject<UpdateFlightTaskRequest>(requestBody),
                    TaskType.Planning => JsonConvert.DeserializeObject<UpdatePlanningTaskRequest>(requestBody),
                    TaskType.Other => JsonConvert.DeserializeObject<UpdateOtherTaskRequest>(requestBody),
                    _ => null
                };
            }

            if (derivedRequest != null)
            {
                context.Items["task"] = derivedRequest;
            }
        }

        await _next(context);
    }
}

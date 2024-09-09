using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Api.Extensions;

public static class RequestExtensions
{
    public static BaseTaskRequest? GetTaskRequestFromContext(this HttpContext context)
    {
        // Safely retrieves the item from HttpContext.Items and casts it to the desired type
        if (context.Items.TryGetValue("task", out object? item) && item is BaseTaskRequest typedItem)
        {
            return typedItem;
        }
        return null;
    }
}

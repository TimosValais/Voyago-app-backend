using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Requests;

public interface ITaskRequest
{
    TaskType TaskType { get; }
}


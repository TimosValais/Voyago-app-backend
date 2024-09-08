using Voyago.App.Contracts.ValueObjects;

namespace Voyago.App.Contracts.Messages;
public record TaskFileUpdateMessage(Guid TaskId, byte[] File, TaskType Type);

namespace Voyago.App.Contracts.Messages;
public record UserUpdateMessage(string Username, string Email, Guid UserId);

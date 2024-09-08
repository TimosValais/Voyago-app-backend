namespace Voyago.App.Contracts.Messages;
public record UserRegisterMessage(string Username, string Email, Guid UserId);

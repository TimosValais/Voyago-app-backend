namespace Voyago.App.Contracts.Requests;
public record LoginUserRequest(string EmailOrUsername, string Password);
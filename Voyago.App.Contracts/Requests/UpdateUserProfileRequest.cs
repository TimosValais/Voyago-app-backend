namespace Voyago.App.Contracts.Requests;
public record UpdateUserProfileRequest(string Email, string? Name, byte[]? ProfilePicture);


namespace Voyago.App.Contracts.Requests;
public record UpdateUserProfileRequest(Guid Id, string Email, string? Name, byte[]? ProfilePicture);


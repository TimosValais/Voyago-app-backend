namespace Voyago.App.Contracts.Responses;
public record UserProfileResponse(Guid Id, string Email, string? Name, string? ProfilePictureUrl);

namespace Voyago.App.Contracts.Messages;
public record UserProfilePictureUpdateMessage(byte[] ProfilePicture, Guid UserId);

namespace Voyago.App.DataAccessLayer.Entities;
public class UserProfile
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

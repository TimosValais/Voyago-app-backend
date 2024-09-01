using Voyago.App.Contracts.Responses;
using Voyago.App.DataAccessLayer.Entities;

namespace Voyago.App.Api.Mappings;

public static class ContractMappings
{
    public static UserProfileResponse MapToResponse(this UserProfile userProfile)
    {
        return new(
                Id: userProfile.Id,
                Email: userProfile.Email,
                Name: userProfile.Name,
                ProfilePictureUrl: userProfile.ProfilePictureUrl
            );
    }

    public static IEnumerable<UserProfileResponse> MapToResponses(this IEnumerable<UserProfile> userProfiles)
    {
        foreach (UserProfile profile in userProfiles)
        {
            yield return profile.MapToResponse();
        }
    }
}

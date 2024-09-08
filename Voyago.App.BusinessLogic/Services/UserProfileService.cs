using MassTransit;
using Microsoft.Extensions.Logging;
using Voyago.App.Contracts.Messages;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.BusinessLogic.Services;
public class UserProfileService : IUserProfileService, IConsumer<UserRegisterMessage>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(IUserProfileRepository userProfileRepository, ILogger<UserProfileService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisterMessage> context)
    {
        UserRegisterMessage message = context.Message;
        if (message == null)
        {
            return;
        }
        UserProfile userToCreate = new()
        {
            Email = message.Email,
            Id = message.UserId,
            Name = message.Username
        };
        await _userProfileRepository.InsertAsync(userToCreate);
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userProfileRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userProfileRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userProfileRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        try
        {

            Task<UserProfile?>? existingEntity = _userProfileRepository.GetByIdAsync(userProfile.Id, cancellationToken);
            if (existingEntity is null)
            {
                throw new Exception("User doesn't exist");
            }
            return await _userProfileRepository.UpdateAsync(userProfile, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

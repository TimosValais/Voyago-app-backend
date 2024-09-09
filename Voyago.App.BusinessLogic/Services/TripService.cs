using Microsoft.Extensions.Logging;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Repositories;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.BusinessLogic.Services;
public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITripUserRoleRepository _tripUserRoleRepository;
    private readonly ILogger<TripService> _logger;

    public TripService(ITripRepository tripRepository,
                       IUserProfileRepository userProfileRepository,
                       ITripUserRoleRepository tripUserRoleRepository,
                       ILogger<TripService> logger)
    {
        _tripRepository = tripRepository;
        _userProfileRepository = userProfileRepository;
        _tripUserRoleRepository = tripUserRoleRepository;
        _logger = logger;
    }

    public async Task<bool> UpsertUser(Guid userId, Guid tripId, TripRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            UserProfile? user = await _userProfileRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            TripUserRoles? userTripRole = await _tripUserRoleRepository.GetByTripAndUserId(tripId, userId, cancellationToken);
            if (userTripRole == null)
            {
                return await _tripUserRoleRepository.InsertAsync(new()
                {
                    Role = role,
                    TripId = tripId,
                    UserId = userId,
                }, cancellationToken);
            }
            else
            {
                return await _tripUserRoleRepository.UpdateAsync(new()
                {
                    Role = role,
                    TripId = tripId,
                    UserId = userId,
                }, cancellationToken);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellation = default)
    {
        try
        {
            return await _tripRepository.DeleteAsync(id, cancellation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tripRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tripRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _tripRepository.GetAllByUserIdAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> RemoveUser(Guid userId, Guid tripId, CancellationToken cancellationToken = default)
    {
        try
        {
            UserProfile? user = await _userProfileRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            TripUserRoles? userTripRole = await _tripUserRoleRepository.GetByTripAndUserId(tripId, userId, cancellationToken);
            if (userTripRole is null) return false;
            return await _tripUserRoleRepository.DeleteAsync(userTripRole.TripId, userTripRole.UserId, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> UpsertAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        try
        {
            Trip? existingEntity = await _tripRepository.GetByIdAsync(trip.Id, cancellationToken);
            if (existingEntity is not null)
            {
                return await _tripRepository.UpdateAsync(trip, cancellationToken);
            }
            return await _tripRepository.InsertAsync(trip, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Repositories;

namespace Voyago.App.BusinessLogic.Services;
public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripService> _logger;

    public TripService(ITripRepository tripRepository, ILogger<TripService> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
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

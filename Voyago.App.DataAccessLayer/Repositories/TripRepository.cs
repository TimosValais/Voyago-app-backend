using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Repositories;

public class TripRepository : ITripRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<TripRepository> _logger;

    public TripRepository(IDbConnectionFactory dbConnectionFactory, ILogger<TripRepository> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<bool> InsertAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Trip (Id, Name, Budget, TripStatus, `From`, `To`)
            VALUES (@Id, @Name, @Budget, @TripStatus, @From, @To);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = trip.Id,
            Name = trip.Name,
            Budget = trip.Budget,
            TripStatus = (int)trip.TripStatus,
            From = trip.From,
            To = trip.To
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Trip
            SET Name = @Name,
                Budget = @Budget,
                TripStatus = @TripStatus,
                `From` = @From,
                `To` = @To
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = trip.Id,
            Name = trip.Name,
            Budget = trip.Budget,
            TripStatus = (int)trip.TripStatus,
            From = trip.From,
            To = trip.To
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Trip
            SET TripStatus = @DeletedStatus
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = id,
            DeletedStatus = (int)TripStatus.Deleted
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT t.*, tur.*, u.*
        FROM Trip t
        LEFT JOIN TripUserRoles tur ON t.Id = tur.TripId
        LEFT JOIN UserProfile u ON tur.UserId = u.Id
        WHERE t.TripStatus != @ExcludeStatus;
    ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, Trip> tripDictionary = [];

        IEnumerable<Trip> trips = await connection.QueryAsync<Trip, TripUserRoles, UserProfile, Trip>(
            sql,
            (trip, tripUserRole, userProfile) =>
            {
                if (!tripDictionary.TryGetValue(trip.Id, out Trip? currentTrip))
                {
                    currentTrip = trip;
                    currentTrip.TripUsers = [];
                    tripDictionary.Add(currentTrip.Id, currentTrip);
                }

                if (tripUserRole != null)
                {
                    tripUserRole.User = userProfile;
                    ((List<TripUserRoles>)currentTrip.TripUsers).Add(tripUserRole);
                }

                return currentTrip;
            },
            new { ExcludeStatus = (int)TripStatus.Deleted },
            splitOn: "UserId,Id"
        );

        return trips.Distinct();
    }

    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT t.*, tur.*, u.*
        FROM Trip t
        LEFT JOIN TripUserRoles tur ON t.Id = tur.TripId
        LEFT JOIN UserProfile u ON tur.UserId = u.Id
        WHERE t.Id = @Id AND t.TripStatus != @ExcludeStatus;
    ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Trip? trip = null;

        await connection.QueryAsync<Trip, TripUserRoles, UserProfile, Trip>(
            sql,
            (t, tripUserRole, userProfile) =>
            {
                if (trip == null)
                {
                    trip = t;
                    trip.TripUsers = [];
                }

                if (tripUserRole != null)
                {
                    tripUserRole.User = userProfile;
                    ((List<TripUserRoles>)trip.TripUsers).Add(tripUserRole);
                }

                return trip;
            },
            new { Id = id, ExcludeStatus = (int)TripStatus.Deleted },
            splitOn: "UserId,Id"
        );

        return trip;
    }

    public async Task<IEnumerable<Trip>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Step 1: Fetch trips associated with the user
        const string tripsSql = @"
        SELECT t.*
        FROM Trip t
        LEFT JOIN TripUserRoles tur ON t.Id = tur.TripId
        WHERE tur.UserId = @UserId AND t.TripStatus != @ExcludeStatus;
    ";

        // Step 2: Fetch all TripUserRoles for the trips fetched in Step 1
        const string tripUsersSql = @"
        SELECT tur.*, u.*
        FROM TripUserRoles tur
        LEFT JOIN UserProfile u ON tur.UserId = u.Id
        WHERE tur.TripId IN @TripIds;
    ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        // Step 1: Fetch all trips for the user
        List<Trip> trips = (await connection.QueryAsync<Trip>(tripsSql, new { UserId = userId, ExcludeStatus = (int)TripStatus.Deleted }))
                        .ToList();

        if (!trips.Any())
        {
            _logger.LogInformation("No trips found for user: {UserId}", userId);
            return Enumerable.Empty<Trip>();
        }

        // Step 2: Fetch all user roles for the fetched trips
        List<Guid> tripIds = trips.Select(t => t.Id).ToList();
        IEnumerable<TripUserRoles> tripUserRoles = await connection.QueryAsync<TripUserRoles, UserProfile, TripUserRoles>(
            tripUsersSql,
            (tripUserRole, userProfile) =>
            {
                tripUserRole.User = userProfile; // Link the UserProfile to TripUserRoles
                return tripUserRole;
            },
            new { TripIds = tripIds },
            splitOn: "Id"
        );

        // Step 3: Fetch tasks from all task tables for the fetched trips
        List<TripTask> tasks = new();

        // Fetch tasks from FlightBookingTask table
        const string flightBookingTaskSql = @"
        SELECT * FROM FlightBookingTask WHERE TripId IN @TripIds;
    ";
        IEnumerable<FlightBookingTask> flightTasks = await connection.QueryAsync<FlightBookingTask>(flightBookingTaskSql, new { TripIds = tripIds });
        tasks.AddRange(flightTasks);

        // Fetch tasks from GeneralBookingTask table
        const string generalBookingTaskSql = @"
        SELECT * FROM GeneralBookingTask WHERE TripId IN @TripIds;
    ";
        IEnumerable<GeneralBookingTask> generalTasks = await connection.QueryAsync<GeneralBookingTask>(generalBookingTaskSql, new { TripIds = tripIds });
        tasks.AddRange(generalTasks);

        // Fetch tasks from HotelBookingTask table
        const string hotelBookingTaskSql = @"
        SELECT * FROM HotelBookingTask WHERE TripId IN @TripIds;
    ";
        IEnumerable<HotelBookingTask> hotelTasks = await connection.QueryAsync<HotelBookingTask>(hotelBookingTaskSql, new { TripIds = tripIds });
        tasks.AddRange(hotelTasks);

        // Fetch tasks from OtherTask table
        const string otherTaskSql = @"
        SELECT * FROM OtherTask WHERE TripId IN @TripIds;
    ";
        IEnumerable<OtherTask> otherTasks = await connection.QueryAsync<OtherTask>(otherTaskSql, new { TripIds = tripIds });
        tasks.AddRange(otherTasks);

        // Fetch tasks from PlanningTask table
        const string planningTaskSql = @"
        SELECT * FROM PlanningTask WHERE TripId IN @TripIds;
    ";
        IEnumerable<PlanningTask> planningTasks = await connection.QueryAsync<PlanningTask>(planningTaskSql, new { TripIds = tripIds });
        tasks.AddRange(planningTasks);

        // Step 4: Map TripUserRoles and Tasks back to the corresponding trips
        Dictionary<Guid, Trip> tripDictionary = trips.ToDictionary(t => t.Id);

        foreach (TripUserRoles tripUserRole in tripUserRoles)
        {
            if (tripDictionary.TryGetValue(tripUserRole.TripId, out Trip? trip))
            {
                trip.TripUsers ??= [];
                trip.TripUsers.Add(tripUserRole);
                _logger.LogInformation("Added User: {UserId} to Trip: {TripId}", tripUserRole.UserId, trip.Id);
            }
            else
            {
                _logger.LogWarning("TripUserRole found for non-existent trip: {TripId}", tripUserRole.TripId);
            }
        }

        foreach (TripTask tripTask in tasks)
        {
            if (tripDictionary.TryGetValue(tripTask.TripId, out Trip? trip))
            {
                trip.Tasks ??= [];
                trip.Tasks.Add(tripTask);
                _logger.LogInformation("Added Task to Trip: {TripId}", trip.Id);
            }
            else
            {
                _logger.LogWarning("Task found for non-existent trip: {TripId}", tripTask.TripId);
            }
        }

        return tripDictionary.Values;
    }




}

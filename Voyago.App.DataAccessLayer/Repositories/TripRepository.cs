using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Repositories;

public class TripRepository : ITripRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TripRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Trip (Id, Budget, TripStatus, From, To)
            VALUES (@Id, @Budget, @TripStatus, @From, @To);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = trip.Id,
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
            SET Budget = @Budget,
                TripStatus = @TripStatus,
                From = @From,
                To = @To
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = trip.Id,
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
            SELECT t.*, tur.*
            FROM Trip t
            LEFT JOIN TripUserRoles tur ON t.Id = tur.TripId
            WHERE t.TripStatus != @ExcludeStatus;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, Trip> tripDictionary = [];

        IEnumerable<Trip> trips = await connection.QueryAsync<Trip, TripUserRoles, Trip>(
            sql,
            (trip, tripUserRole) =>
            {
                if (!tripDictionary.TryGetValue(trip.Id, out Trip? currentTrip))
                {
                    currentTrip = trip;
                    currentTrip.TripUsers = [];
                    tripDictionary.Add(currentTrip.Id, currentTrip);
                }

                if (tripUserRole != null)
                {
                    ((List<TripUserRoles>)currentTrip.TripUsers).Add(tripUserRole);
                }

                return currentTrip;
            },
            new { ExcludeStatus = (int)TripStatus.Deleted },
            splitOn: "UserId"
        );

        return trips.Distinct();
    }

    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT t.*, tur.*
            FROM Trip t
            LEFT JOIN TripUserRoles tur ON t.Id = tur.TripId
            WHERE t.Id = @Id AND t.TripStatus != @ExcludeStatus;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Trip? trip = null;

        await connection.QueryAsync<Trip, TripUserRoles, Trip>(
            sql,
            (t, tripUserRole) =>
            {
                if (trip == null)
                {
                    trip = t;
                    trip.TripUsers = [];
                }

                if (tripUserRole != null)
                {
                    ((List<TripUserRoles>)trip.TripUsers).Add(tripUserRole);
                }

                return trip;
            },
            new { Id = id, ExcludeStatus = (int)TripStatus.Deleted },
            splitOn: "UserId"
        );

        return trip;
    }

    public async Task<IEnumerable<Trip>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT t.*, tur.*
            FROM Trip t
            INNER JOIN TripUserRoles tur ON t.Id = tur.TripId
            WHERE tur.UserId = @UserId AND t.TripStatus != @ExcludeStatus;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, Trip> tripDictionary = [];

        IEnumerable<Trip> trips = await connection.QueryAsync<Trip, TripUserRoles, Trip>(
            sql,
            (trip, tripUserRole) =>
            {
                if (!tripDictionary.TryGetValue(trip.Id, out Trip? currentTrip))
                {
                    currentTrip = trip;
                    currentTrip.TripUsers = [];
                    tripDictionary.Add(currentTrip.Id, currentTrip);
                }

                if (tripUserRole != null)
                {
                    ((List<TripUserRoles>)currentTrip.TripUsers).Add(tripUserRole);
                }

                return currentTrip;
            },
            new { UserId = userId, ExcludeStatus = (int)TripStatus.Deleted },
            splitOn: "UserId"
        );

        return trips.Distinct();
    }
}

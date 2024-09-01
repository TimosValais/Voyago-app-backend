using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

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
            TripStatus = (int)trip.Tripstatus, // Convert enum to integer for database storage
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
            TripStatus = (int)trip.Tripstatus, // Convert enum to integer for database storage
            From = trip.From,
            To = trip.To
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM Trip WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM Trip;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Trip>(sql);
    }

    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM Trip
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<Trip>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Trip>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                t.Id, 
                t.Budget,
                t.TripStatus,
                t.From,
                t.To
            FROM Trip t
            INNER JOIN TripUserRoles tur ON t.Id = tur.TripId
            WHERE tur.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Trip>(sql, new { UserId = userId });
    }
}

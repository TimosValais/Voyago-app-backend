using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

internal class TripUserRoleRepository : ITripUserRoleRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TripUserRoleRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<TripUserRoles?> GetByTripAndUserId(Guid tripId, Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                UserId, 
                TripId, 
                Role
            FROM TripUserRoles
            WHERE TripId = @TripId AND UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        var parameters = new { TripId = tripId, UserId = userId };
        return await connection.QueryFirstOrDefaultAsync<TripUserRoles>(sql, parameters);
    }

    public async Task<IEnumerable<TripUserRoles>> GetByTripId(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                UserId, 
                TripId, 
                Role
            FROM TripUserRoles
            WHERE TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<TripUserRoles>(sql, new { TripId = tripId });
    }

    public async Task<bool> InsertAsync(TripUserRoles tripUserRoles, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO TripUserRoles (UserId, TripId, Role)
            VALUES (@UserId, @TripId, @Role);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        var parameters = new
        {
            UserId = tripUserRoles.UserId,
            TripId = tripUserRoles.TripId,
            Role = (int)tripUserRoles.Role // Convert enum to integer for database storage
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(TripUserRoles tripUserRoles, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE TripUserRoles
            SET Role = @Role
            WHERE UserId = @UserId AND TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        var parameters = new
        {
            UserId = tripUserRoles.UserId,
            TripId = tripUserRoles.TripId,
            Role = (int)tripUserRoles.Role // Convert enum to integer for database storage
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }
}

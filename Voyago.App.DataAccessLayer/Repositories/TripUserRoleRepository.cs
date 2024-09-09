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

    public async Task<bool> DeleteAsync(Guid tripId, Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE 
            FROM TripUserRoles
            WHERE TripId = @TripId AND UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new { TripId = tripId, UserId = userId };
        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<TripUserRoles?> GetByTripAndUserId(Guid tripId, Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                tur.UserId, 
                tur.TripId, 
                tur.Role,
                u.Id, u.Email, u.Name, u.ProfilePictureUrl,
                t.Id, t.Name, t.Budget, t.TripStatus, t.From, t.To
            FROM TripUserRoles tur
            INNER JOIN UserProfile u ON tur.UserId = u.Id
            INNER JOIN Trip t ON tur.TripId = t.Id
            WHERE tur.TripId = @TripId AND tur.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new { TripId = tripId, UserId = userId };
        IEnumerable<TripUserRoles> result = await connection.QueryAsync<TripUserRoles, UserProfile, Trip, TripUserRoles>(
            sql,
            (tripUserRole, user, trip) =>
            {
                tripUserRole.User = user;
                tripUserRole.Trip = trip;
                return tripUserRole;
            },
            parameters,
            splitOn: "Id,Id"
        );
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<TripUserRoles>> GetByTripId(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                tur.UserId, 
                tur.TripId, 
                tur.Role,
                u.Id, u.Email, u.Name, u.ProfilePictureUrl,
                t.Id, t.Name, t.Budget, t.TripStatus, t.From, t.To
            FROM TripUserRoles tur
            INNER JOIN UserProfile u ON tur.UserId = u.Id
            INNER JOIN Trip t ON tur.TripId = t.Id
            WHERE tur.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TripUserRoles, UserProfile, Trip, TripUserRoles>(
            sql,
            (tripUserRole, user, trip) =>
            {
                tripUserRole.User = user;
                tripUserRole.Trip = trip;
                return tripUserRole;
            },
            new { TripId = tripId },
            splitOn: "Id,Id"
        );
    }

    public async Task<IEnumerable<TripUserRoles>> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                tur.UserId, 
                tur.TripId, 
                tur.Role,
                u.Id, u.Email, u.Name, u.ProfilePictureUrl,
                t.Id, t.Name, t.Budget, t.TripStatus, t.From, t.To
            FROM TripUserRoles tur
            INNER JOIN UserProfile u ON tur.UserId = u.Id
            INNER JOIN Trip t ON tur.TripId = t.Id
            WHERE tur.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TripUserRoles, UserProfile, Trip, TripUserRoles>(
            sql,
            (tripUserRole, user, trip) =>
            {
                tripUserRole.User = user;
                tripUserRole.Trip = trip;
                return tripUserRole;
            },
            new { UserId = userId },
            splitOn: "Id,Id"
        );
    }

    public async Task<bool> InsertAsync(TripUserRoles tripUserRoles, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO TripUserRoles (UserId, TripId, Role)
            VALUES (@UserId, @TripId, @Role);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
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

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
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

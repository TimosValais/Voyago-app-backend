using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserProfileRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO UserProfile (Id, Email, Name, ProfilePictureUrl)
            VALUES (@Id, @Email, @Name, @ProfilePictureUrl);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = userProfile.Id,
            Email = userProfile.Email,
            Name = userProfile.Name,
            ProfilePictureUrl = userProfile.ProfilePictureUrl
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE UserProfile
            SET Email = @Email,
                Name = @Name,
                ProfilePictureUrl = @ProfilePictureUrl
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = userProfile.Id,
            Email = userProfile.Email,
            Name = userProfile.Name,
            ProfilePictureUrl = userProfile.ProfilePictureUrl
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM UserProfile WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                Email, 
                Name, 
                ProfilePictureUrl
            FROM UserProfile;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<UserProfile>(sql);
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                Email, 
                Name, 
                ProfilePictureUrl
            FROM UserProfile
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { Id = id });
    }

    public async Task<IEnumerable<UserProfile>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                up.Id, 
                up.Email, 
                up.Name, 
                up.ProfilePictureUrl
            FROM UserProfile up
            INNER JOIN TripTaskUser ttu ON up.Id = ttu.UserId
            WHERE ttu.TaskId = @TaskId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<UserProfile>(sql, new { TaskId = taskId });
    }

    public async Task<IEnumerable<UserProfile>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                up.Id, 
                up.Email, 
                up.Name, 
                up.ProfilePictureUrl
            FROM UserProfile up
            INNER JOIN TripUserRoles tur ON up.Id = tur.UserId
            WHERE tur.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<UserProfile>(sql, new { TripId = tripId });
    }

    public async Task<UserProfile?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                Email, 
                Name, 
                ProfilePictureUrl
            FROM UserProfile
            WHERE Name = @Name;
        "
        ;

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { Name = username });
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                Email, 
                Name, 
                ProfilePictureUrl
            FROM UserProfile
            WHERE Email = @Email;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { Email = email });
    }
}

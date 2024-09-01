using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;
using Voyago.App.DataAccessLayer.ValueObjects;

namespace Voyago.App.DataAccessLayer.Repositories;

public class TaskUserRepository : ITaskUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TaskUserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(TaskUser taskUser, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO TaskUser (UserId, TaskId)
            VALUES (@UserId, @TaskId);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            UserId = taskUser.UserId,
            TaskId = taskUser.TaskId
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(TaskUser taskUser, CancellationToken cancellationToken = default)
    {
        // Assuming an update operation may involve changing UserId or TaskId
        const string sql = @"
            UPDATE TaskUser
            SET UserId = @UserId,
                TaskId = @TaskId
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            UserId = taskUser.UserId,
            TaskId = taskUser.TaskId,
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(TaskUser taskUser, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM TaskUser
            WHERE UserId = @UserId AND TaskId = @TaskId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            UserId = taskUser.UserId,
            TaskId = taskUser.TaskId
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<IEnumerable<TripTask>> GetTasksFromUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                tt.* 
            FROM TaskUser tu
            INNER JOIN TripTask tt ON tu.TaskId = tt.TaskId
            WHERE tu.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TripTask>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserProfile>> GetUsersFromTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                up.* 
            FROM TaskUser tu
            INNER JOIN UserProfile up ON tu.UserId = up.Id
            WHERE tu.TaskId = @TaskId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<UserProfile>(sql, new { TaskId = taskId });
    }
}

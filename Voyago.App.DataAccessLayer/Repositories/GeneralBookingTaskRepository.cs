using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class GeneralBookingTaskRepository : IGeneralBookingTaskRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GeneralBookingTaskRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(GeneralBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO GeneralBookingTask (Id, TripId, Type, Status, Deadline, Description, Name, MoneySpent, DocumentsUrls, Notes)
            VALUES (@Id, @TripId, @Type, @Status, @Deadline, @Description, @Name, @MoneySpent, @DocumentsUrls, @Notes);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = task.Id,
            TripId = task.TripId,
            Type = (int)task.Type,
            Status = (int)task.Status,
            Deadline = task.Deadline,
            Description = task.Description,
            Name = task.Name,
            MoneySpent = task.MoneySpent,
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null,
            Notes = task.Notes != null ? System.Text.Json.JsonSerializer.Serialize(task.Notes) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(GeneralBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE GeneralBookingTask
            SET TripId = @TripId,
                Type = @Type,
                Status = @Status,
                Deadline = @Deadline,
                Description = @Description,
                Name = @Name,
                MoneySpent = @MoneySpent,
                DocumentsUrls = @DocumentsUrls,
                Notes = @Notes
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = task.Id,
            TripId = task.TripId,
            Type = (int)task.Type,
            Status = (int)task.Status,
            Deadline = task.Deadline,
            Description = task.Description,
            Name = task.Name,
            MoneySpent = task.MoneySpent,
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null,
            Notes = task.Notes != null ? System.Text.Json.JsonSerializer.Serialize(task.Notes) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM GeneralBookingTask WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<GeneralBookingTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT gbt.*, up.*
            FROM GeneralBookingTask gbt
            LEFT JOIN TaskUser tu ON gbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, GeneralBookingTask> taskDictionary = [];

        IEnumerable<GeneralBookingTask> tasks = await connection.QueryAsync<GeneralBookingTask, UserProfile, GeneralBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out GeneralBookingTask? currentTask))
                {
                    currentTask = task;
                    currentTask.Users = [];
                    taskDictionary.Add(currentTask.Id, currentTask);
                }

                if (user != null)
                {
                    ((List<UserProfile>)currentTask.Users).Add(user);
                }

                return currentTask;
            },
            splitOn: "Id"
        );

        return tasks.Distinct();
    }

    public async Task<GeneralBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT gbt.*, up.*
            FROM GeneralBookingTask gbt
            LEFT JOIN TaskUser tu ON gbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE gbt.Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        GeneralBookingTask? task = null;

        await connection.QueryAsync<GeneralBookingTask, UserProfile, GeneralBookingTask>(
            sql,
            (gbt, user) =>
            {
                if (task == null)
                {
                    task = gbt;
                    task.Users = [];
                }

                if (user != null)
                {
                    ((List<UserProfile>)task.Users).Add(user);
                }

                return task;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return task;
    }

    public async Task<IEnumerable<GeneralBookingTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT gbt.*, up.*
            FROM GeneralBookingTask gbt
            LEFT JOIN TaskUser tu ON gbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE gbt.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, GeneralBookingTask> taskDictionary = [];

        IEnumerable<GeneralBookingTask> tasks = await connection.QueryAsync<GeneralBookingTask, UserProfile, GeneralBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out GeneralBookingTask? currentTask))
                {
                    currentTask = task;
                    currentTask.Users = [];
                    taskDictionary.Add(currentTask.Id, currentTask);
                }

                if (user != null)
                {
                    ((List<UserProfile>)currentTask.Users).Add(user);
                }

                return currentTask;
            },
            new { TripId = tripId },
            splitOn: "Id"
        );

        return tasks.Distinct();
    }

    public async Task<IEnumerable<GeneralBookingTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT gbt.*, up.*
            FROM GeneralBookingTask gbt
            INNER JOIN TaskUser tu ON gbt.Id = tu.TaskId
            INNER JOIN UserProfile up ON tu.UserId = up.Id
            WHERE tu.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, GeneralBookingTask> taskDictionary = [];

        IEnumerable<GeneralBookingTask> tasks = await connection.QueryAsync<GeneralBookingTask, UserProfile, GeneralBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out GeneralBookingTask? currentTask))
                {
                    currentTask = task;
                    currentTask.Users = [];
                    taskDictionary.Add(currentTask.Id, currentTask);
                }

                if (user != null)
                {
                    ((List<UserProfile>)currentTask.Users).Add(user);
                }

                return currentTask;
            },
            new { UserId = userId },
            splitOn: "Id"
        );

        return tasks.Distinct();
    }
}
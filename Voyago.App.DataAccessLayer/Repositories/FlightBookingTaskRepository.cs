using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class FlightBookingTaskRepository : IFlightBookingTaskRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public FlightBookingTaskRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(FlightBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO FlightBookingTask (Id, DepartureDate, ReturnDate, TripId, Type, Status, Deadline, Description, Name, MoneySpent, DocumentsUrls)
            VALUES (@Id, @DepartureDate, @ReturnDate, @TripId, @Type, @Status, @Deadline, @Description, @Name, @MoneySpent, @DocumentsUrls);
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = task.Id,
            DepartureDate = task.DepartureDate,
            ReturnDate = task.ReturnDate,
            TripId = task.TripId,
            Type = (int)task.Type,
            Status = (int)task.Status,
            Deadline = task.Deadline,
            Description = task.Description,
            Name = task.Name,
            MoneySpent = task.MoneySpent,
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(FlightBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE FlightBookingTask
            SET DepartureDate = @DepartureDate, 
                ReturnDate = @ReturnDate, 
                Status = @Status, 
                Deadline = @Deadline, 
                Description = @Description, 
                Name = @Name, 
                MoneySpent = @MoneySpent, 
                DocumentsUrls = @DocumentsUrls
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new
        {
            Id = task.Id,
            DepartureDate = task.DepartureDate,
            ReturnDate = task.ReturnDate,
            Status = (int)task.Status,
            Deadline = task.Deadline,
            Description = task.Description,
            Name = task.Name,
            MoneySpent = task.MoneySpent,
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM FlightBookingTask WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<FlightBookingTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT fbt.*, up.*
            FROM FlightBookingTask fbt
            LEFT JOIN TaskUser tu ON fbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, FlightBookingTask> taskDictionary = [];

        IEnumerable<FlightBookingTask> tasks = await connection.QueryAsync<FlightBookingTask, UserProfile, FlightBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out FlightBookingTask? currentTask))
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

    public async Task<FlightBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT fbt.*, up.*
            FROM FlightBookingTask fbt
            LEFT JOIN TaskUser tu ON fbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE fbt.Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        FlightBookingTask? task = null;

        await connection.QueryAsync<FlightBookingTask, UserProfile, FlightBookingTask>(
            sql,
            (fbt, user) =>
            {
                if (task == null)
                {
                    task = fbt;
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

    public async Task<IEnumerable<FlightBookingTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT fbt.*, up.*
            FROM FlightBookingTask fbt
            LEFT JOIN TaskUser tu ON fbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE fbt.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, FlightBookingTask> taskDictionary = [];

        IEnumerable<FlightBookingTask> tasks = await connection.QueryAsync<FlightBookingTask, UserProfile, FlightBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out FlightBookingTask? currentTask))
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

    public async Task<IEnumerable<FlightBookingTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT fbt.*, up.*
            FROM FlightBookingTask fbt
            INNER JOIN TaskUser tu ON fbt.Id = tu.TaskId
            INNER JOIN UserProfile up ON tu.UserId = up.Id
            WHERE tu.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, FlightBookingTask> taskDictionary = [];

        IEnumerable<FlightBookingTask> tasks = await connection.QueryAsync<FlightBookingTask, UserProfile, FlightBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out FlightBookingTask? currentTask))
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

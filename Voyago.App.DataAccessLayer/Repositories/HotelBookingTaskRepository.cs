using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class HotelBookingTaskRepository : IHotelBookingTaskRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public HotelBookingTaskRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(HotelBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO HotelBookingTask (Id, TripId, Type, Status, Deadline, Description, Name, MoneySpent, DocumentsUrls, CheckInDate, CheckOutDate, ContactNo)
            VALUES (@Id, @TripId, @Type, @Status, @Deadline, @Description, @Name, @MoneySpent, @DocumentsUrls, @CheckInDate, @CheckOutDate, @ContactNo);
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
            CheckInDate = task.CheckInDate,
            CheckOutDate = task.CheckOutDate,
            ContactNo = task.ContactNo
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(HotelBookingTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE HotelBookingTask
            SET TripId = @TripId,
                Type = @Type,
                Status = @Status,
                Deadline = @Deadline,
                Description = @Description,
                Name = @Name,
                MoneySpent = @MoneySpent,
                DocumentsUrls = @DocumentsUrls,
                CheckInDate = @CheckInDate,
                CheckOutDate = @CheckOutDate,
                ContactNo = @ContactNo
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
            CheckInDate = task.CheckInDate,
            CheckOutDate = task.CheckOutDate,
            ContactNo = task.ContactNo
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM HotelBookingTask WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<HotelBookingTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT hbt.*, up.*
            FROM HotelBookingTask hbt
            LEFT JOIN TaskUser tu ON hbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, HotelBookingTask> taskDictionary = new();

        IEnumerable<HotelBookingTask> tasks = await connection.QueryAsync<HotelBookingTask, UserProfile, HotelBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out HotelBookingTask? currentTask))
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

    public async Task<HotelBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT hbt.*, up.*
            FROM HotelBookingTask hbt
            LEFT JOIN TaskUser tu ON hbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE hbt.Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        HotelBookingTask? task = null;

        await connection.QueryAsync<HotelBookingTask, UserProfile, HotelBookingTask>(
            sql,
            (hbt, user) =>
            {
                if (task == null)
                {
                    task = hbt;
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

    public async Task<IEnumerable<HotelBookingTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT hbt.*, up.*
            FROM HotelBookingTask hbt
            LEFT JOIN TaskUser tu ON hbt.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE hbt.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, HotelBookingTask> taskDictionary = new();

        IEnumerable<HotelBookingTask> tasks = await connection.QueryAsync<HotelBookingTask, UserProfile, HotelBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out HotelBookingTask? currentTask))
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

    public async Task<IEnumerable<HotelBookingTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT hbt.*, up.*
            FROM HotelBookingTask hbt
            INNER JOIN TaskUser tu ON hbt.Id = tu.TaskId
            INNER JOIN UserProfile up ON tu.UserId = up.Id
            WHERE tu.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, HotelBookingTask> taskDictionary = new();

        IEnumerable<HotelBookingTask> tasks = await connection.QueryAsync<HotelBookingTask, UserProfile, HotelBookingTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out HotelBookingTask? currentTask))
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

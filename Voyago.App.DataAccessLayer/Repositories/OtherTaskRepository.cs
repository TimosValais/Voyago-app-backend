﻿using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class OtherTaskRepository : IOtherTaskRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public OtherTaskRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(OtherTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO OtherTask (Id, TripId, Type, Status, Deadline, Description, Name, MoneySpent, DocumentsUrls)
            VALUES (@Id, @TripId, @Type, @Status, @Deadline, @Description, @Name, @MoneySpent, @DocumentsUrls);
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
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(OtherTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE OtherTask
            SET TripId = @TripId,
                Type = @Type,
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM OtherTask WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<OtherTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT ot.*, up.*
            FROM OtherTask ot
            LEFT JOIN TaskUser tu ON ot.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, OtherTask> taskDictionary = [];

        IEnumerable<OtherTask> tasks = await connection.QueryAsync<OtherTask, UserProfile, OtherTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out OtherTask? currentTask))
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

    public async Task<OtherTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT ot.*, up.*
            FROM OtherTask ot
            LEFT JOIN TaskUser tu ON ot.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE ot.Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        OtherTask? task = null;

        await connection.QueryAsync<OtherTask, UserProfile, OtherTask>(
            sql,
            (ot, user) =>
            {
                if (task == null)
                {
                    task = ot;
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

    public async Task<IEnumerable<OtherTask>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT ot.*, up.*
            FROM OtherTask ot
            LEFT JOIN TaskUser tu ON ot.Id = tu.TaskId
            LEFT JOIN UserProfile up ON tu.UserId = up.Id
            WHERE ot.TripId = @TripId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, OtherTask> taskDictionary = [];

        IEnumerable<OtherTask> tasks = await connection.QueryAsync<OtherTask, UserProfile, OtherTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out OtherTask? currentTask))
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

    public async Task<IEnumerable<OtherTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT ot.*, up.*
            FROM OtherTask ot
            INNER JOIN TaskUser tu ON ot.Id = tu.TaskId
            INNER JOIN UserProfile up ON tu.UserId = up.Id
            WHERE tu.UserId = @UserId;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        Dictionary<Guid, OtherTask> taskDictionary = [];

        IEnumerable<OtherTask> tasks = await connection.QueryAsync<OtherTask, UserProfile, OtherTask>(
            sql,
            (task, user) =>
            {
                if (!taskDictionary.TryGetValue(task.Id, out OtherTask? currentTask))
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

using Dapper;
using System.Data;
using Voyago.App.DataAccessLayer.Entities;
using Voyago.App.DataAccessLayer.Extensions;

namespace Voyago.App.DataAccessLayer.Repositories;

public class PlanningTaskRepository : IPlanningTaskRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PlanningTaskRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> InsertAsync(PlanningTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO PlanningTask (Id, TripId, Type, Status, Deadline, Description, Name, MoneySpent, DocumentsUrls, Steps)
            VALUES (@Id, @TripId, @Type, @Status, @Deadline, @Description, @Name, @MoneySpent, @DocumentsUrls, @Steps);
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
            Steps = task.Steps != null ? System.Text.Json.JsonSerializer.Serialize(task.Steps) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> UpdateAsync(PlanningTask task, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE PlanningTask
            SET TripId = @TripId,
                Type = @Type,
                Status = @Status,
                Deadline = @Deadline,
                Description = @Description,
                Name = @Name,
                MoneySpent = @MoneySpent,
                DocumentsUrls = @DocumentsUrls,
                Steps = @Steps
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
            Steps = task.Steps != null ? System.Text.Json.JsonSerializer.Serialize(task.Steps) : null
        };

        int result = await connection.ExecuteAsync(sql, parameters);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM PlanningTask WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(sql, new { Id = id });
        return result > 0;
    }

    public async Task<IEnumerable<PlanningTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM PlanningTask;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<PlanningTask>(sql);
    }

    public async Task<PlanningTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM PlanningTask
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<PlanningTask>(sql, new { Id = id });
    }
}

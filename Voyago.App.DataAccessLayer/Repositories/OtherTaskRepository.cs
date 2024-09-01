using Dapper;
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
            SELECT *
            FROM OtherTask;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<OtherTask>(sql);
    }

    public async Task<OtherTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM OtherTask
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<OtherTask>(sql, new { Id = id });
    }
}

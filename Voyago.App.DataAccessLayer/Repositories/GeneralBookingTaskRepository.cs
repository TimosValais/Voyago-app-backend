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
            SELECT *
            FROM GeneralBookingTask;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<GeneralBookingTask>(sql);
    }

    public async Task<GeneralBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM GeneralBookingTask
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<GeneralBookingTask>(sql, new { Id = id });
    }
}

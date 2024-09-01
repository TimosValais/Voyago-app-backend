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
            DocumentsUrls = task.DocumentsUrls != null ? System.Text.Json.JsonSerializer.Serialize(task.DocumentsUrls) : null // Store as JSON
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
            SELECT 
                Id, 
                DepartureDate, 
                ReturnDate, 
                TripId, 
                Type, 
                Status, 
                Deadline, 
                Description, 
                Name, 
                MoneySpent, 
                DocumentsUrls
            FROM FlightBookingTask;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<FlightBookingTask>(sql);
    }

    public async Task<FlightBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, 
                DepartureDate, 
                ReturnDate, 
                TripId, 
                Type, 
                Status, 
                Deadline, 
                Description, 
                Name, 
                MoneySpent, 
                DocumentsUrls
            FROM FlightBookingTask
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<FlightBookingTask>(sql, new { Id = id });
    }
}

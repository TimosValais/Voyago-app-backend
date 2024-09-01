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
            SELECT *
            SELECT *
            FROM HotelBookingTask;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<HotelBookingTask>(sql);
    }

    public async Task<HotelBookingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT *
            FROM HotelBookingTask
            WHERE Id = @Id;
        ";

        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<HotelBookingTask>(sql, new { Id = id });
    }
}

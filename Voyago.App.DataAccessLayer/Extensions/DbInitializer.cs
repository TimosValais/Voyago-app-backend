using Dapper;
using System.Data;

namespace Voyago.App.DataAccessLayer.Extensions;
public class DbInitializer : IDbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        List<Task> createTasks =
        [
            CreateUserProfileTableAsync(connection),
            CreateTripTableAsync(connection),
            CreateTripUserRolesTableAsync(connection),
            CreateFlightBookingTaskTableAsync(connection),
            CreateGeneralBookingTaskTableAsync(connection),
            CreateHotelBookingTaskTableAsync(connection),
            CreateOtherTaskTableAsync(connection),
            CreatePlanningTaskTableAsync(connection),
            CreateTaskUserTableAsync(connection)
        ];

        await Task.WhenAll(createTasks);
    }

    private async Task CreateUserProfileTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS UserProfile (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL,
                Name TEXT,
                ProfilePictureUrl TEXT
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateTripTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Trip (
            Id TEXT PRIMARY KEY,
            Budget REAL NOT NULL,
            TripStatus INTEGER NOT NULL,
            From DATETIME NOT NULL,
            To DATETIME NOT NULL
        );
    ";
        await connection.ExecuteAsync(sql);
    }



    private async Task CreateTripUserRolesTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS TripUserRoles (
                UserId TEXT NOT NULL,
                TripId TEXT NOT NULL,
                Role INTEGER NOT NULL,
                PRIMARY KEY (UserId, TripId),
                FOREIGN KEY (UserId) REFERENCES UserProfile(Id),
                FOREIGN KEY (TripId) REFERENCES Trip(Id)
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateFlightBookingTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS FlightBookingTask (
                Id TEXT PRIMARY KEY,
                TripId TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Deadline DATETIME NOT NULL,
                Description TEXT,
                Name TEXT NOT NULL,
                MoneySpent REAL NOT NULL,
                DocumentsUrls TEXT,
                DepartureDate DATETIME NOT NULL,
                ReturnDate DATETIME NOT NULL
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateGeneralBookingTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS GeneralBookingTask (
                Id TEXT PRIMARY KEY,
                TripId TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Deadline DATETIME NOT NULL,
                Description TEXT,
                Name TEXT NOT NULL,
                MoneySpent REAL NOT NULL,
                DocumentsUrls TEXT,
                Notes TEXT
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateHotelBookingTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS HotelBookingTask (
                Id TEXT PRIMARY KEY,
                TripId TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Deadline DATETIME,
                Description TEXT,
                Name TEXT NOT NULL,
                MoneySpent REAL NOT NULL,
                DocumentsUrls TEXT,
                CheckInDate DATETIME,
                CheckOutDate DATETIME,
                ContactNo TEXT
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateOtherTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS OtherTask (
                Id TEXT PRIMARY KEY,
                TripId TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Deadline DATETIME NOT NULL,
                Description TEXT,
                Name TEXT NOT NULL,
                MoneySpent REAL NOT NULL,
                DocumentsUrls TEXT
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreatePlanningTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS PlanningTask (
                Id TEXT PRIMARY KEY,
                TripId TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Deadline DATETIME NOT NULL,
                Description TEXT,
                Name TEXT NOT NULL,
                MoneySpent REAL NOT NULL,
                DocumentsUrls TEXT,
                Steps TEXT
            );
        ";
        await connection.ExecuteAsync(sql);
    }
    private async Task CreateTaskUserTableAsync(IDbConnection connection)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS TaskUser (
                UserId TEXT NOT NULL,
                TaskId TEXT NOT NULL,
                PRIMARY KEY (UserId, TaskId),
                FOREIGN KEY (UserId) REFERENCES UserProfile(Id)
            );
        ";
        await connection.ExecuteAsync(sql);
    }
}
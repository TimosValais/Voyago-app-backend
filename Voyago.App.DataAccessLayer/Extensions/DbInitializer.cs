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

        await CreateUserProfileTableAsync(connection);
        await CreateTripTableAsync(connection);
        await CreateTripUserRolesTableAsync(connection);
        await CreateFlightBookingTaskTableAsync(connection);
        await CreateGeneralBookingTaskTableAsync(connection);
        await CreateHotelBookingTaskTableAsync(connection);
        await CreateOtherTaskTableAsync(connection);
        await CreatePlanningTaskTableAsync(connection);
        await CreateTaskUserTableAsync(connection);
    }

    private async Task CreateUserProfileTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS UserProfile (
            Id CHAR(36) PRIMARY KEY,  
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
            Id CHAR(36) PRIMARY KEY,  
            Budget DECIMAL NOT NULL,
            TripStatus INTEGER NOT NULL,
            `From` DATETIME NOT NULL,
            `To` DATETIME NOT NULL
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateTripUserRolesTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS TripUserRoles (
            UserId CHAR(36) NOT NULL,
            TripId CHAR(36) NOT NULL,
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
            Id CHAR(36) PRIMARY KEY,  
            TripId CHAR(36) NOT NULL,
            Type INTEGER NOT NULL,
            Status INTEGER NOT NULL,
            Deadline DATETIME NOT NULL,
            Description TEXT,
            Name TEXT NOT NULL,
            MoneySpent DECIMAL NOT NULL,
            DocumentsUrls TEXT,
            DepartureDate DATETIME NOT NULL,
            ReturnDate DATETIME NOT NULL,
            FOREIGN KEY (TripId) REFERENCES Trip(Id)
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateGeneralBookingTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS GeneralBookingTask (
            Id CHAR(36) PRIMARY KEY,  
            TripId CHAR(36) NOT NULL,
            Type INTEGER NOT NULL,
            Status INTEGER NOT NULL,
            Deadline DATETIME NOT NULL,
            Description TEXT,
            Name TEXT NOT NULL,
            MoneySpent DECIMAL NOT NULL,
            DocumentsUrls TEXT,
            Notes TEXT,
            FOREIGN KEY (TripId) REFERENCES Trip(Id)
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateHotelBookingTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS HotelBookingTask (
            Id CHAR(36) PRIMARY KEY,  
            TripId CHAR(36) NOT NULL,
            Type INTEGER NOT NULL,
            Status INTEGER NOT NULL,
            Deadline DATETIME,
            Description TEXT,
            Name TEXT NOT NULL,
            MoneySpent DECIMAL NOT NULL,
            DocumentsUrls TEXT,
            CheckInDate DATETIME,
            CheckOutDate DATETIME,
            ContactNo TEXT,
            FOREIGN KEY (TripId) REFERENCES Trip(Id)
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateOtherTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS OtherTask (
            Id CHAR(36) PRIMARY KEY,  
            TripId CHAR(36) NOT NULL,
            Type INTEGER NOT NULL,
            Status INTEGER NOT NULL,
            Deadline DATETIME NOT NULL,
            Description TEXT,
            Name TEXT NOT NULL,
            MoneySpent DECIMAL NOT NULL,
            DocumentsUrls TEXT,
            FOREIGN KEY (TripId) REFERENCES Trip(Id)
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreatePlanningTaskTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS PlanningTask (
            Id CHAR(36) PRIMARY KEY,  
            TripId CHAR(36) NOT NULL,
            Type INTEGER NOT NULL,
            Status INTEGER NOT NULL,
            Deadline DATETIME NOT NULL,
            Description TEXT,
            Name TEXT NOT NULL,
            MoneySpent DECIMAL NOT NULL,
            DocumentsUrls TEXT,
            Steps TEXT,
            FOREIGN KEY (TripId) REFERENCES Trip(Id)
        );
    ";
        await connection.ExecuteAsync(sql);
    }

    private async Task CreateTaskUserTableAsync(IDbConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS TaskUser (
            UserId CHAR(36) NOT NULL,
            TaskId CHAR(36) NOT NULL,
            PRIMARY KEY (UserId, TaskId),
            FOREIGN KEY (UserId) REFERENCES UserProfile(Id),
            FOREIGN KEY (TaskId) REFERENCES FlightBookingTask(Id) -- Assuming this references tasks
        );
    ";
        await connection.ExecuteAsync(sql);
    }

}

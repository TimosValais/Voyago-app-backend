using Dapper;
using System.Data;

namespace Voyago.Auth.DataAccessLayer.Extensions;
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
        await CreateUserTableAsync(connection);
    }

    private async Task CreateUserTableAsync(IDbConnection connection)
    {
        string sql = @"
                CREATE TABLE IF NOT EXISTS User (
                    Id TEXT PRIMARY KEY,
                    Username TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL
                );
            ";
        await connection.ExecuteAsync(sql);
    }
}

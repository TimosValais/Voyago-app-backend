using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;

namespace Voyago.App.DataAccessLayer.Extensions;
public class SqlLiteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlLiteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        InitializeSQLiteProvider();
    }

    private void InitializeSQLiteProvider()
    {
        // Initialize the SQLite provider
        Batteries.Init();
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

using Microsoft.Data.Sqlite;
using System.Data;

namespace Voyago.App.DataAccessLayer.Extensions;
public class SqlLiteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlLiteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}

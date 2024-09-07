using System.Data;

namespace Voyago.Auth.DataAccessLayer.Extensions;
public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}
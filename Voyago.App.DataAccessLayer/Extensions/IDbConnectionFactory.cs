using System.Data;

namespace Voyago.App.DataAccessLayer.Extensions;
public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

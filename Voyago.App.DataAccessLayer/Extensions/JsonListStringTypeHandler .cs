using Dapper;
using System.Data;
using System.Text.Json;

namespace Voyago.App.DataAccessLayer.Extensions;

public class JsonListStringTypeHandler : SqlMapper.TypeHandler<IEnumerable<string>>
{
    // Method to serialize the list of strings to JSON when writing to the database
    public override void SetValue(IDbDataParameter parameter, IEnumerable<string>? value)
    {
        parameter.Value = value != null ? JsonSerializer.Serialize(value) : (object)DBNull.Value;
        parameter.DbType = DbType.String;
    }

    // Method to deserialize JSON back to the list of strings when reading from the database
    public override IEnumerable<string> Parse(object value)
    {
        return value != null ? JsonSerializer.Deserialize<IEnumerable<string>>(value.ToString()!) ?? [] : [];
    }
}

using Dapper;
using System.Data;

namespace Voyago.Auth.DataAccessLayer.Extensions;
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString("D"); // Convert GUID to string format (D format)
        parameter.DbType = DbType.String;
    }

    public override Guid Parse(object value)
    {
        if (value is string stringValue)
        {
            return Guid.Parse(stringValue); // Convert string back to GUID
        }

        throw new ArgumentException("Invalid value for GUID parsing", nameof(value));
    }
}

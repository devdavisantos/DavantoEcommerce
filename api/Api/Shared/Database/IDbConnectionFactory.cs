using System.Data.Common;

namespace Api.Shared.Database
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection();
    }
}
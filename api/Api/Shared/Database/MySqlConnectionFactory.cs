using System.Data.Common;
using MySqlConnector;

namespace Api.Shared.Database
{
    public sealed class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public MySqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }
        public DbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
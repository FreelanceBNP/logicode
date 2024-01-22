using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LGC.BNP.MIKUNI.Web.Models
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
        Task<string> givingConnectionString();
    }
    public class ConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString) => _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var Connection = new SqlConnection(_connectionString);
            await Connection.OpenAsync();
            return Connection;
        }

        public async Task<string> givingConnectionString()
        {
            return _connectionString;
        }
    }
}

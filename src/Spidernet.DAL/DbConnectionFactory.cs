using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;

namespace Spidernet.DAL
{
    public class DbConnectionFactory
    {
        private IOptions<DbConnectionConfig> config { get; set; }

        private IDbConnection connection;

        /// <summary>
        /// 主
        /// </summary>
        public IDbConnection Connection
        {

            get
            {
                if (connection == null)
                {
                    connection = new MySqlConnection(config.Value.ConnectionString);
                }
                return connection;
            }
        }

    }
}

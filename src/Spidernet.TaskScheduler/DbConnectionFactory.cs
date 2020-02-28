using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Spidernet.TaskScheduler
{
    internal class DbConnectionFactory
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

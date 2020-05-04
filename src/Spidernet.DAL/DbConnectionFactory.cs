using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace Spidernet.DAL {
  /// <summary>
  /// 
  /// </summary>
  public class DbConnectionFactory {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public DbConnectionFactory(IOptions<DbConnectionConfig> config) {
      this.config = config;
    }
    /// <summary>
    /// 
    /// </summary>
    private IOptions<DbConnectionConfig> config { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private IDbConnection connection;

    /// <summary>
    /// 数据库链接
    /// </summary>
    public IDbConnection Connection {
      get {
        if (connection == null) {
          connection = new NpgsqlConnection(config.Value.ConnectionString);
        }
        return connection;
      }
    }

  }
}

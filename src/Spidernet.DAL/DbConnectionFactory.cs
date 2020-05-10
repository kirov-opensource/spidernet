using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Npgsql;
using Spidernet.DAL.CustomSqlMapper;
using Spidernet.Model.Models;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Spidernet.DAL {
  /// <summary>
  /// 
  /// </summary>
  public class DbConnectionFactory {
    private static readonly object _initialLock = new object();
    private static bool _initialized = false;
    /// <summary>
    /// 进行一些全局的初始化
    /// </summary>
    public void InitialGlobalSettings() {
      if (!_initialized) {
        lock (_initialLock) {
          if (!_initialized) {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(null, new[] { typeof(PropertyParsingRuleModel) });
            SqlMapper.AddTypeHandler(new JSONTypeHandler<PropertyParsingRuleModel>());
            SqlMapper.AddTypeHandler(new JSONTypeHandler<JObject>());
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    public DbConnectionFactory(IOptions<DbConnectionConfig> config) {
      if (!_initialized) {
        this.InitialGlobalSettings();
      }
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

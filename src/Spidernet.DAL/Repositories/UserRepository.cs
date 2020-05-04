using Dapper;
using Snowflake.Core;
using Spidernet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spidernet.DAL.Repositories {
  /// <summary>
  /// 用户
  /// </summary>
  public class UserRepository : Repository<User> {
    /// <summary>
    /// 用户
    /// </summary>
    /// <param name="dbConnectionFactory"></param>
    /// <param name="idWorker"></param>
    public UserRepository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) : base(dbConnectionFactory, idWorker) {
    }

    /// <summary>
    /// 根据Token查询用户
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<User> GetByToken(string token) {
      var queriedData = await Connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM {TableName} WHERE token = @token AND delete_at IS NULL", new { token });
      return queriedData;
    }
  }
}

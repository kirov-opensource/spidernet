using Dapper;
using Snowflake.Core;
using Spidernet.DAL.Entities;
using System.Threading.Tasks;

namespace Spidernet.DAL.Repositories {
  /// <summary>
  /// 
  /// </summary>
  public class TemplateRepository : Repository<Template> {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbConnectionFactory"></param>
    /// <param name="idWorker"></param>
    public TemplateRepository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) : base(dbConnectionFactory, idWorker) {

    }

    /// <summary>
    /// 根据编号获取数据
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public async Task<Template> GetByNo(string no) {
      var quriedTemplate = Connection.QueryFirstOrDefault<Template>($"SELECT * FROM {TableName} WHERE no = @no AND delete_at IS NULL", new { no });
      return quriedTemplate;
    }
  }
}

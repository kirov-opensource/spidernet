using Dapper;
using Dapper.Contrib.Extensions;
using Snowflake.Core;
using Spidernet.DAL.Entities;
using Spidernet.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Spidernet.DAL.Repositories {
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class Repository<TEntity> where TEntity : Entity, new() {
    /// <summary>
    /// 
    /// </summary>
    protected readonly IdWorker idWorker;

    /// <summary>
    /// 默认60S的数据库执行超时时间
    /// </summary>
    protected readonly int commandTimeout = 60;

    /// <summary>
    /// 
    /// </summary>
    private DbConnectionFactory dbConnectionFactory;

    /// <summary>
    /// 
    /// </summary>
    protected IDbConnection Connection { get { return dbConnectionFactory.Connection; } }

    /// <summary>
    /// 表名称
    /// </summary>
    protected string TableName => GetTableName(EntityType);

    /// <summary>
    /// 主键名称
    /// </summary>
    protected string PrimaryKeyName => GetPrimaryKeyName(EntityType);

    private Type EntityType => typeof(TEntity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbConnectionFactory"></param>
    /// <param name="idWorker"></param>
    public Repository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker) {
      this.dbConnectionFactory = dbConnectionFactory;
      this.idWorker = idWorker;
    }

    /// <summary>
    /// 插入对象
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<long> InsertAsync(TEntity entity) {
      if (entity.id == 0) {
        entity.id = idWorker.NextId();
      }
      return await Connection.InsertAsync(entity, commandTimeout: commandTimeout);
    }

    /// <summary>
    /// 更新对象
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<bool> UpdateAsync(TEntity entity) {
      entity.update_at = DateTime.Now;
      return await Connection.UpdateAsync(entity, commandTimeout: commandTimeout);
    }

    /// <summary>
    /// 软删除
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<bool> SoftDeleteAsync(TEntity entity) {
      entity.delete_at = DateTime.Now;
      return await UpdateAsync(entity);
    }

    /// <summary>
    /// 物理删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual async Task<bool> DeleteAsync(long id) {
      return await Connection.DeleteAsync(new TEntity { id = id });
    }

    /// <summary>
    /// 根据ID集合删除所有符合条件的对象
    /// </summary>
    /// <param name="primaryKeyValues"></param>
    /// <returns></returns>
    public virtual async Task<bool> DeleteAsync(IList<long> primaryKeyValues) {
      if (primaryKeyValues == null || !primaryKeyValues.Any()) {
        return true;
      }
      string sql = $"delete from {TableName} where {PrimaryKeyName} = ANY(@primaryKeyValues)";
      return await Connection.ExecuteAsync(sql, new { primaryKeyValues }) > 0;
    }

    /// <summary>
    /// 根据Id获取对象
    /// </summary>
    /// <param name="primaryKeyValue"></param>
    /// <returns></returns>
    public virtual async Task<TEntity> FirstOrDefaultAsync(long primaryKeyValue) {
      string sql = $"select * from {TableName} where {PrimaryKeyName} = @primaryKeyValue and delete_at is null";
      return await Connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { primaryKeyValue });
    }

    /// <summary>
    /// 获取对象数量
    /// </summary>
    /// <returns></returns>
    public virtual async Task<int> CountAsync(string whereString = null, object parameters = null, bool ignoreDeletedData = true) {
      string sql = $"select count(1) from {TableName}";
      if (!ignoreDeletedData) {
        if (!string.IsNullOrEmpty(whereString)) {
          sql = $"{sql} where {whereString}";
        }
      } else {
        sql = $"{sql} where {AttachCondition(whereString)}";
      }

      return await Connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    /// <summary>
    /// Sum
    /// </summary>
    /// <param name="sumColumn"></param>
    /// <param name="whereString"></param>
    /// <param name="parameters"></param>
    /// <param name="ignoreDeletedData"></param>
    /// <returns></returns>
    protected virtual async Task<int> SumAsync(string sumColumn, string whereString = null, object parameters = null, bool ignoreDeletedData = true) {
      string sql = $"select sum({sumColumn}) from {TableName}";
      if (!ignoreDeletedData) {
        if (!string.IsNullOrEmpty(whereString)) {
          sql = $"{sql} where {whereString}";
        }
      } else {
        sql = $"{sql} where {AttachCondition(whereString)}";
      }
      return await Connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    /// <summary>  
    /// 获取表名  
    /// </summary>  
    /// <param name="type"></param>  
    /// <returns></returns>  
    private static string GetTableName(Type type) {
      string tableName = null;
      object[] attributes = type.GetCustomAttributes(false);
      foreach (var attr in attributes) {
        if (attr is TableAttribute) {
          var tableAttribute = attr as TableAttribute;
          return tableAttribute.Name;
        }
      }
      return tableName.ToSnakeCase();
    }

    private static string GetPrimaryKeyName(Type type) {
      foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
        var attributes = field.FieldType.GetCustomAttributes(true);
        foreach (var attribute in attributes) {
          if (attribute is ExplicitKeyAttribute) {
            return field.Name;
          }
        }

      }
      throw new ArgumentException("Entity has no primary key.");
    }

    /// <summary>
    /// 附加查询条件
    /// </summary>
    /// <param name="queryString">原始查询条件</param>
    /// <returns></returns>
    private string AttachCondition(string queryString) {
      if (string.IsNullOrEmpty(queryString)) {
        queryString = "delete_at is null";
      } else {
        if (queryString.Trim().StartsWith("and ")) {
          queryString = queryString.Remove(0, 4);
        }

        queryString = $"{queryString} and delete_at is null";
      }

      return queryString;
    }

  }
}

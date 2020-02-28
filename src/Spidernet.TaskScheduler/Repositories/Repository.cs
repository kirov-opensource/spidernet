using Dapper;
using Dapper.Contrib.Extensions;
using Snowflake.Core;
using Spidernet.TaskScheduler.Entities;
using Spidernet.TaskScheduler.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Spidernet.TaskScheduler.Repositories
{
    internal abstract class Repository<TEntity> where TEntity : Entity, new()
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IdWorker idWorker;
        //默认60S的数据库执行超时时间
        private readonly int commandTimeout = 60;

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
        public string TableName => GetTableName(EntityType);

        /// <summary>
        /// 主键名称
        /// </summary>
        public string PrimaryKeyName => GetPrimaryKeyName(EntityType);

        private Type EntityType => typeof(TEntity);


        internal Repository(DbConnectionFactory dbConnectionFactory, IdWorker idWorker)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this.idWorker = idWorker;
        }


        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns></returns>
        internal long Insert(TEntity entity)
        {
            if (entity.id == 0)
            {
                entity.id = idWorker.NextId();
            }
            return Connection.Insert(entity, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal bool Update(TEntity entity)
        {
            entity.update_at = DateTime.Now;
            return Connection.Update(entity, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="id">实体Id</param>
        /// <returns></returns>
        internal virtual bool Delete(long id)
        {
            return Connection.Delete(new TEntity { id = id });
        }

        /// <summary>
        /// 根据ID集合删除所有符合条件的对象
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        internal virtual bool Delete(IList<object> primaryKeyValues)
        {
            if (primaryKeyValues == null || !primaryKeyValues.Any())
            {
                return true;
            }
            string sql = $"delete from {TableName} where {PrimaryKeyName} = ANY(@primaryKeyValues)";
            return Connection.Execute(sql, new { primaryKeyValues }) > 0;
        }

        /// <summary>
        /// 根据Id获取对象
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>对象</returns>
        internal virtual TEntity FirstOrDefault(object primaryKeyValue)
        {
            string sql = $"select * from {TableName} where {PrimaryKeyName} = @primaryKeyValue and delete_at is null";
            return Connection.QueryFirstOrDefault<TEntity>(sql, new { primaryKeyValue });
        }

        /// <summary>
        /// 获取对象数量
        /// </summary>
        /// <returns></returns>
        internal virtual int Count(string whereString = null, object parameters = null, bool ignoreDeletedData = true)
        {
            string sql = $"select count(1) from {TableName}";
            if (!ignoreDeletedData)
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    sql = $"{sql} where {whereString}";
                }
            }
            else
            {
                sql = $"{sql} where {AttachCondition(whereString)}";
            }

            return Connection.ExecuteScalar<int>(sql, parameters);
        }

        /// <summary>
        /// Sum
        /// </summary>
        /// <param name="sumColumn"></param>
        /// <param name="whereString"></param>
        /// <param name="parameters"></param>
        /// <param name="ignoreDeletedData"></param>
        /// <returns></returns>
        protected virtual int Sum(string sumColumn, string whereString = null, object parameters = null, bool ignoreDeletedData = true)
        {
            string sql = $"select sum({sumColumn}) from {TableName}";
            if (!ignoreDeletedData)
            {
                if (!string.IsNullOrEmpty(whereString))
                {
                    sql = $"{sql} where {whereString}";
                }
            }
            else
            {
                sql = $"{sql} where {AttachCondition(whereString)}";
            }
            return Connection.ExecuteScalar<int>(sql, parameters);
        }

        /// <summary>  
        /// 获取表名  
        /// </summary>  
        /// <param name="type"></param>  
        /// <returns></returns>  
        private static string GetTableName(Type type)
        {
            string tableName = null;
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                if (attr is TableAttribute)
                {
                    var tableAttribute = attr as TableAttribute;
                    return tableAttribute.Name;
                }
            }
            return tableName.ToSnakeCase();
        }

        private static string GetPrimaryKeyName(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                var attributes = field.FieldType.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    if (attribute is ExplicitKeyAttribute)
                    {
                        var explicitKeyAttribute = attribute as ExplicitKeyAttribute;
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
        private string AttachCondition(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                queryString = "delete_at is null";
            }
            else
            {
                if (queryString.Trim().StartsWith("and "))
                {
                    queryString = queryString.Remove(0, 4);
                }

                queryString = $"{queryString} and delete_at is null";
            }

            return queryString;
        }

    }
}

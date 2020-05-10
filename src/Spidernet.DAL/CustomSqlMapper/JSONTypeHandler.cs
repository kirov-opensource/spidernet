using Dapper;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Spidernet.DAL.CustomSqlMapper {
  public class JSONTypeHandler<T> : SqlMapper.TypeHandler<T> {
    public override T Parse(object value) {
      return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value.ToString());
    }

    public override void SetValue(IDbDataParameter parameter, T value) {
      var param = (Npgsql.NpgsqlParameter)parameter;
      param.NpgsqlDbType = NpgsqlDbType.Json;
      param.Value = value;
    }
  }
}

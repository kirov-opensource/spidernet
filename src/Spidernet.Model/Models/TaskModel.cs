using Spidernet.Model.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Spidernet.Model.Models {
  public class TaskModel {
    /// <summary>
    /// Id
    /// </summary>
    public long TaskId { get; set; }
    /// <summary>
    /// Uri
    /// </summary>
    public string Uri { get; set; }
    /// <summary>
    /// 请求类型
    /// </summary>
    [EnumDataType(typeof(RequestMethodEnum))]
    public RequestMethodEnum RequestMethod { get; set; }
    /// <summary>
    /// 字段解析
    /// </summary>
    public IDictionary<string, PropertyParsingRuleModel> PropertyParsingRules { get; set; }
    /// <summary>
    /// 环境变量
    /// </summary>
    [EnumDataType(typeof(RequestParameterModel))]
    public RequestParameterModel RequestParameter { get; set; }
    /// <summary>
    /// 结果类型
    /// </summary>
    [EnumDataType(typeof(ResponseTypeEnum))]
    public ResponseTypeEnum ResponseType { get; set; }
  }
}

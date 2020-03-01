using Spidernet.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Model.Models {
  public class TaskModel {
    public RequestMethodEnum RequestMethod { get; set; }
    /// <summary>
    /// Id
    /// </summary>
    public long TaskId { get; set; }
    /// <summary>
    /// Uri
    /// </summary>
    public string Uri { get; set; }
    /// <summary>
    /// 解析
    /// </summary>
    public IDictionary<string, PropertyParsingRuleModel> PropertyParsingRules { get; set; }
    /// <summary>
    /// 环境变量
    /// </summary>
    public RequestParameterModel RequestParameter { get; set; }
  }
}

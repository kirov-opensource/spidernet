using Spidernet.Model.Models;
using System.Collections.Generic;

namespace Spidernet.BLL.Models.Templates {
  /// <summary>
  /// 新增模型
  /// </summary>
  public class CreateTemplateModel {
    /// <summary>
    /// 编号，不可重复
    /// </summary>
    public string No { get; set; }
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 转换规则
    /// </summary>
    public Dictionary<string, PropertyParsingRuleModel> PropertyParsingRule { get; set; }
    /// <summary>
    /// 资源地址
    /// </summary>
    public string Uri { get; set; }
  }
}

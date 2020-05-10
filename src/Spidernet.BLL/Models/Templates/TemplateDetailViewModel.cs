using Spidernet.Model.Models;

namespace Spidernet.BLL.Models.Templates {
  /// <summary>
  /// 详细模板模型
  /// </summary>
  public class TemplateDetailViewModel : Model {
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
    public PropertyParsingRuleModel PropertyParsingRule { get; set; }
    /// <summary>
    /// 资源地址
    /// </summary>
    public string Uri { get; set; }
  }
}

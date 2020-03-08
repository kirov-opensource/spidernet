using Spidernet.Model.Enums;
using System.Collections.Generic;

namespace Spidernet.Model.Models {
  public class PropertyParsingRuleModel {
    /// <summary>
    /// 类型
    /// </summary>
    public OutputTypeEnum Type { get; set; }
    /// <summary>
    /// 节点选择器
    /// </summary>
    public SelectorModel NodeSelector { get; set; }
    /// <summary>
    /// Parser
    /// </summary>
    public IDictionary<string, PropertyParsingRuleModel> PropertyParsingRules { get; set; }
    /// <summary>
    /// 输出类型
    /// </summary>
    public OutputFromEnum OutputFrom { get; set; }
    /// <summary>
    /// 输出特性
    /// 当<see cref="OutputFrom"/>为<see cref="OutputFromEnum.Attribute"/>时该字段生效
    /// </summary>
    public string OutputFromAttributeName { get; set; }

  }
}

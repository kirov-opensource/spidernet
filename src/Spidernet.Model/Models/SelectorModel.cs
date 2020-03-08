using Spidernet.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Spidernet.Model.Models {
  public class SelectorModel {
    /// <summary>
    /// 选择器类型
    /// </summary>
    [EnumDataType(typeof(SelectorEnum))]
    public SelectorEnum Type { get; set; }
    /// <summary>
    /// 选择表达式
    /// </summary>
    public string MatchExpression { get; set; }
  }
}

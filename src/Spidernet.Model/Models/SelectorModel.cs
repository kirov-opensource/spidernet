using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Model.Models {
  public class SelectorModel {
    /// <summary>
    /// 选择器类型
    /// </summary>
    public Enums.SelectorEnum Type { get; set; }
    /// <summary>
    /// 选择表达式
    /// </summary>
    public string MatchExpression { get; set; }
  }
}

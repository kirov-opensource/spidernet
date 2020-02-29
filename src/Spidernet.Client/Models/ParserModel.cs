using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Client.Models {
  public class ParserModel {
    /// <summary>
    /// 类型
    /// </summary>
    public Enums.Parser Type { get; set; }
    /// <summary>
    /// 节点选择器
    /// </summary>
    public SelectorModel Selector { get; set; }
    /// <summary>
    /// Parser
    /// </summary>
    public IDictionary<string, ParserModel> Parser { get; set; }
  }
}

using Spidernet.Client.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
    public SelectorModel NodeSelector { get; set; }
    /// <summary>
    /// Parser
    /// </summary>
    public IDictionary<string, ParserModel> Parser { get; set; }
    /// <summary>
    /// 结果输出
    /// </summary>
    public OutputFrom OutputFrom { get; set; }
    /// <summary>
    /// 输入类型
    /// </summary>
    public string OutputFromAttributeName { get; set; }

  }
}

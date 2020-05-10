using Spidernet.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spidernet.BLL.Models.Templates {
  public class UpdateTemplateModel {
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

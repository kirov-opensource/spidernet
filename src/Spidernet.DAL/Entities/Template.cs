using Newtonsoft.Json.Linq;
using Spidernet.Model.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spidernet.DAL.Entities {
  [Table("\"template\"")]
  public class Template : Entity {
    /// <summary>
    /// 编号
    /// </summary>
    public string no { get; set; }

    /// <summary>
    /// 头
    /// </summary>
    [Column(TypeName = "json")]
    public JObject header { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "json")]
    public PropertyParsingRuleModel property_parsing_rule { get; set; }

    /// <summary>
    /// 具有后续任务的属性
    /// </summary>
    [Column(TypeName = "json")]
    public JObject subsequent_task_property_scheme { get; set; }

    /// <summary>
    /// 目标地址
    /// </summary>
    public string uri { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long user_id { get; set; }
  }
}

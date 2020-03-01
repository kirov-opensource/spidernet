namespace Spidernet.DAL.Entities {
  public class Template : Entity {
    /// <summary>
    /// 编号
    /// </summary>
    public string no { get; set; }

    /// <summary>
    /// 头
    /// </summary>
    public string header { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string property_parsing_rule { get; set; }

    /// <summary>
    /// 具有后续任务的属性
    /// </summary>
    public string subsequent_task_property_scheme { get; set; }

    public string uri { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long user_id { get; set; }

  }
}

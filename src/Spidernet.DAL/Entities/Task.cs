using System;

namespace Spidernet.DAL.Entities {
  public class Task : Entity {

    /// <summary>
    /// 模板ID
    /// </summary>
    public long template_id { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    public string http_method { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    public string parameters { get; set; }

    /// <summary>
    /// 头
    /// </summary>
    public string header { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    public long parent_id { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public long user_id { get; set; }

    /// <summary>
    /// 任务id
    /// </summary>
    public long job_id { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string stage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string uri { get; set; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public DateTime? execute_at { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? complete_at { get; set; }

    /// <summary>
    /// 失败
    /// </summary>
    public DateTime? fail_at { get; set; }

    /// <summary>
    /// 执行结果数据ID
    /// </summary>
    public long result_id { get; set; }

  }
}

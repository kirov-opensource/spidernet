using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spidernet.Client.Models {
  public class TaskModel {
    /// <summary>
    /// Id
    /// </summary>
    public long TaskId { get; set; }
    /// <summary>
    /// Uri
    /// </summary>
    public string Uri { get; set; }
    /// <summary>
    /// 解析
    /// </summary>
    public IDictionary<string,PropertyModel> Properties { get; set; }
    /// <summary>
    /// 环境变量
    /// </summary>
    public IDictionary<string,string> Variables { get; set; }
  }
}

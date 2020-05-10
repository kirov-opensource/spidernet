using System;

namespace Spidernet.BLL.Models {
  public class Model {
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateAt { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateAt { get; set; }
  }
}

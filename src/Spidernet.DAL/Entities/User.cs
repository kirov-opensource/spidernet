namespace Spidernet.DAL.Entities {
  /// <summary>
  /// 用户信息
  /// </summary>
  public class User : Entity {
    /// <summary>
    /// 邮箱
    /// </summary>
    public string email { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 昵称
    /// </summary>
    public string nick_name { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string password { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string mobile_number { get; set; }
    /// <summary>
    /// Token
    /// </summary>
    public string token { get; set; }
  }
}

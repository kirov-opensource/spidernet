namespace Spidernet.BLL.Models.Users {
  public class UserModel : Model {
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 别名
    /// </summary>
    public string NickName { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
  }
}

namespace Spidernet.DAL.Entities {
  public class User : Entity {
    public string email { get; set; }
    public string name { get; set; }
    public string nick_name { get; set; }
    public string password { get; set; }
    public string mobile_number { get; set; }
    public string token { get; set; }
  }
}

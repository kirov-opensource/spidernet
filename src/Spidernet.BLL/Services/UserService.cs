using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spidernet.BLL.Models.Users;

namespace Spidernet.BLL.Services {
  public class UserService : Service {
    public UserService(ILoggerFactory loggerFactory, IStringLocalizerFactory stringLocalizerFactory, Session session) : base(loggerFactory, stringLocalizerFactory, session) {

    }

    public UserModel GetByToken(string token) {
      return default;
    }
  }
}

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spidernet.BLL.Models.Users;
using Spidernet.DAL.Repositories;
using System.Threading.Tasks;

namespace Spidernet.BLL.Services {
  /// <summary>
  /// 
  /// </summary>
  public class UserService : Service {
    private readonly UserRepository userRepository;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="userRepository"></param>
    /// <param name="session"></param>
    public UserService(
      ILoggerFactory loggerFactory,
      UserRepository userRepository,
      Session session) : base(loggerFactory, null, session) {
      this.userRepository = userRepository;
    }
    /// <summary>
    /// 根据Token获取，Session需要，这一步可以增加缓存以提高性能
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<UserModel> GetByToken(string token) {
      var queriedData = await userRepository.GetByToken(token);
      if (queriedData != null) {
        return new UserModel {
          Email = queriedData.email,
          CreateAt = queriedData.create_at,
          Id = queriedData.id,
          Name = queriedData.name,
          NickName = queriedData.nick_name,
          Token = queriedData.token,
          UpdateAt = queriedData.update_at
        };
      }
      return null;
    }
    /// <summary>
    /// 根据ID获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserModel> GetById(long id) {
      var queriedData = await userRepository.FirstOrDefaultAsync(1000000000000);
      if (queriedData != null) {
        return new UserModel {
          Email = queriedData.email,
          CreateAt = queriedData.create_at,
          Id = queriedData.id,
          Name = queriedData.name,
          NickName = queriedData.nick_name,
          Token = queriedData.token,
          UpdateAt = queriedData.update_at
        };
      }
      return null;
    }
  }
}

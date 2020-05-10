using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Spidernet.BLL.Services {
  public class Service {
    /// <summary>
    /// 
    /// </summary>
    protected readonly ILogger logger;

    private readonly IStringLocalizer localizer;

    /// <summary>
    /// 当前会话信息
    /// </summary>
    protected readonly Session session;

    /// <summary>
    /// 
    /// </summary>
    public Service(ILoggerFactory loggerFactory,
      IStringLocalizerFactory stringLocalizerFactory,
      Session session) {
      logger = loggerFactory.CreateLogger(GetType().FullName);
      //localizer = stringLocalizerFactory.Create(typeof(Service));
      this.session = session;
    }
  }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;
using Spidernet.WebAPI.Filters;

namespace Spidernet.WebAPI.Controllers.V1 {

  [ServiceFilter(typeof(AuthorizationFilter))]
  [Route("api/[controller]")]
  [ApiController]
  public class BaseController : ControllerBase {
    protected readonly ILogger logger;
    protected readonly Session session;
    public BaseController(ILoggerFactory logger, Session session) {
      this.logger = logger.CreateLogger(GetType().FullName);
      this.session = session;
    }
  }
}
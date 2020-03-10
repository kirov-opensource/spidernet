using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;

namespace Spidernet.WebAPI.Controllers.V1 {
  [Route("api/[controller]")]
  [ApiController]
  public class SessionController : BaseController {
    public SessionController(ILoggerFactory logger, Session session) : base(logger, session) {
    }
  }
}
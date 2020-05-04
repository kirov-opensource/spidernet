using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;
using Spidernet.BLL.Services;
using Spidernet.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace Spidernet.WebAPI.Controllers.V1 {
  [Route("api/[controller]")]
  [ApiController]
  public class SessionController : BaseController {
    private UserService userService;

    public SessionController(ILoggerFactory logger, Session session, UserService userService) : base(logger, session) {
      this.userService = userService;
    }
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [Route("user_info")]
    [HttpGet]
    public async Task<IActionResult> UserInfo() {
      return Ok(session.User);
    }
  }
}
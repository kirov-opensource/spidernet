using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;
using Spidernet.BLL.Services;
using System.Net;
using System.Threading.Tasks;

namespace Spidernet.WebAPI.Filters {
  public class AuthorizationFilter : IAsyncAuthorizationFilter {
    private readonly UserService userService;
    private readonly Session session;
    private readonly ILogger logger;

    public AuthorizationFilter(UserService userService, Session session, ILoggerFactory loggerFactory) {
      this.userService = userService;
      this.session = session;
      this.logger = loggerFactory.CreateLogger(GetType().FullName);
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {

      var endpoint = context.HttpContext.GetEndpoint();

      if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) {
        return;
      }

      string userToken = context.HttpContext.Request.Cookies["user_token"];

      if (!string.IsNullOrEmpty(userToken)) {
        session.User = await userService.GetByToken(userToken);
        return;
      } else {
        session.User = await userService.GetById(1000000000000L);
        return;
      }

      context.Result = new ObjectResult("用户未授权") { StatusCode = (int)HttpStatusCode.Unauthorized };

    }
  }
}

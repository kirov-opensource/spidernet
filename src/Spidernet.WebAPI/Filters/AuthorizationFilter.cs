using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;
using Spidernet.BLL.Services;

namespace Spidernet.WebAPI.Filters {
  public class AuthorizationFilter : IAuthorizationFilter {
    private readonly UserService userService;
    private readonly Session session;
    private readonly ILogger logger;

    public AuthorizationFilter(UserService userService, Session session, ILoggerFactory loggerFactory) {
      this.userService = userService;
      this.session = session;
      this.logger = loggerFactory.CreateLogger(GetType().FullName);
    }

    public void OnAuthorization(AuthorizationFilterContext context) {
      //身份认证通过
      var endpoint = context.HttpContext.GetEndpoint();
      if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) {
        return true;
      }

      string userToken = context.HttpContext.Request.Cookies["user_token"];
      if (!string.IsNullOrEmpty(userToken)) {
        session.User = userService.GetByToken(userToken);
        return;
      }
      context.Result = new ObjectResult("用户未授权") { StatusCode = (int)HttpStatusCode.Unauthorized };

    }
  }
}

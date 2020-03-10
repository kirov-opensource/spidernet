using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spidernet.Extension.Exceptions;
using Spidernet.WebAPI.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Spidernet.WebAPI.Extensions {
  public class ExceptionHandler {
    private readonly RequestDelegate _next;
    private readonly ILogger logger;

    public ExceptionHandler(RequestDelegate next, ILoggerFactory loggerFactory) {
      _next = next;
      this.logger = loggerFactory.CreateLogger(GetType().FullName);
    }

    public async Task Invoke(HttpContext context) {
      try {
        await _next.Invoke(context);
      } catch (Exception ex) {
        await HandleExceptionAsync(context, ex);
      }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
      var httpResponse = context.Response;
      httpResponse.ContentType = "application/json";
      switch (exception) {
        case NoPermissionException _:
          httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
          await httpResponse.WriteAsync(JsonConvert.SerializeObject(new MessageModel {
            Code = "PERMISSION_EXCEPTION",
            Message = "未授权"
          }));
          break;
        case BusinessException _:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          await httpResponse.WriteAsync(JsonConvert.SerializeObject(new MessageModel {
            Code = "BUSINESS_EXCEPTION",
            Message = exception?.Message
          }));
          break;
        case ArgumentNullException _:
        case ArgumentException _:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          await httpResponse.WriteAsync(JsonConvert.SerializeObject(new MessageModel {
            Code = "ARGUMENT_EXCEPTION",
            Message = exception?.Message
          }));
          break;
        default:
          httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
          logger.LogError("ExceptionHandler:HandleExceptionAsync",
             $"错误信息：{exception.Message}\n错误堆栈：{exception.StackTrace}\nInnerException.Message：{exception.InnerException?.Message}", $"请求头：{context?.Request?.Headers?.ToString()}\nStatusCode：{httpResponse.StatusCode}");
          await httpResponse.WriteAsync(JsonConvert.SerializeObject(new MessageModel {
            Code = "SYSTEM_EXCEPTION",
            Message = $"ServerError：{exception?.Message}"
          }));
          break;
      }
    }
  }
}

// Adapted from https://weblog.west-wind.com/posts/2016/oct/16/error-handling-and-exceptionfilter-dependency-injection-for-aspnet-core-apis

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace McsaMeetsMailer.ErrorHandling
{
  public class ApiExceptionFilter : ExceptionFilterAttribute
  {
    public override void OnException(ExceptionContext context)
    {
      ApiError apiError = null;

      if (context.Exception is ApiException)
      {
        // handle explicit 'known' API errors
        var ex = context.Exception as ApiException;
        context.Exception = null;
        apiError = new ApiError(ex.Message);
        //apiError.Errors = ex.Errors;

        context.HttpContext.Response.StatusCode = ex.StatusCode;
      }
      else if (context.Exception is UnauthorizedAccessException)
      {
        apiError = new ApiError("Unauthorized Access");
        context.HttpContext.Response.StatusCode = 401;

        // handle logging here
      }
      else
      {
        // Unhandled errors
#if !DEBUG
        var msg = "An unhandled error occurred.";                
        string stack = null;
#else
        var msg = context.Exception.GetBaseException().Message;
        string stack = context.Exception.StackTrace;
#endif

        apiError = new ApiError(msg);
        apiError.Detail = stack;

        context.HttpContext.Response.StatusCode = 500;

        // handle logging here
      }

      // always return a JSON result
      context.Result = new JsonResult(apiError);

      base.OnException(context);
    }
  }
}
// Adapted from https://weblog.west-wind.com/posts/2016/oct/16/error-handling-and-exceptionfilter-dependency-injection-for-aspnet-core-apis

using System;

namespace McsaMeetsMailer.ErrorHandling
{
  public class ApiException : Exception
  {
    public int StatusCode { get; set; }
    //public ValidationErrorCollection Errors { get; set; }

    public ApiException(
      string message,
      int statusCode = 500/*,
      ValidationErrorCollection errors = null*/)
      :
      base(message)
    {
      StatusCode = statusCode;
      //Errors = errors;
    }

    public ApiException(
      Exception ex,
      int statusCode = 500)
      :
      base(ex.Message)
    {
      StatusCode = statusCode;
    }
  }
}

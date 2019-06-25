using System;

namespace McsaMeetsMailer.Utils.RestRequest
{
  public class RestRequestException : Exception
  {
    public RestRequestException(
      in string message,
      in Exception exception)
    :
      base(message, exception)
    {
    }
  }
}

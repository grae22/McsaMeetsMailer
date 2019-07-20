using System;

namespace McsaMeetsMailer.Services.Exceptions
{
  public class MeetsServiceException : Exception
  {
    public MeetsServiceException(
      in string message,
      in Exception exception = null)
    :
      base(message, exception)
    {
    }
  }
}

using System;

namespace McsaMeetsMailer.Utils.Validation
{
  public static class CommonValidation
  {
    public static void RaiseExceptionIfArgumentNull(in object argument, in string argumentName)
    {
      if (argument != null)
      {
        return;
      }

      throw new ArgumentNullException(argumentName);
    }
  }
}

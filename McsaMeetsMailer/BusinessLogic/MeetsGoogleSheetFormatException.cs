using System;

namespace McsaMeetsMailer.BusinessLogic
{
  public class MeetsGoogleSheetFormatException : Exception
  {
    public MeetsGoogleSheetFormatException(in string message)
      :
      base(message)
    {
    }
  }
}

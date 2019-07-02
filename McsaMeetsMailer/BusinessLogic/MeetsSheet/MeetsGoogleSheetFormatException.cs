using System;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
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

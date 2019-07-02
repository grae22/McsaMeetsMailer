using System;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public class EmailAddressGoogleSheetFormatException : Exception
  {
    public EmailAddressGoogleSheetFormatException(in string message)
      :
      base(message)
    {
    }
  }
}

using System;

using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public static class MeetSheetValueValidatorFactory
  {
    public static IValidator CreateValidator(in string columnHeaderText)
    {
      if (columnHeaderText.Contains("date", StringComparison.OrdinalIgnoreCase))
      {
        return new DateValidator();
      }

      if (columnHeaderText.Contains("email", StringComparison.OrdinalIgnoreCase))
      {
        return new EmailValidator();
      }

      return new NullValidator();
    }
  }
}

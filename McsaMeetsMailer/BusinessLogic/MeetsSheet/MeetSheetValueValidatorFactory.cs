using System;

using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public static class MeetSheetValueValidatorFactory
  {
    public static IValidatorChain CreateValidator(in MeetField meetField)
    {
      var validatorChain = new ValidatorChain();

      if (meetField.IsRequired)
      {
        validatorChain.AddValidator(new ValueRequiredValidator());
      }

      if (meetField.RawText.Contains("date", StringComparison.OrdinalIgnoreCase))
      {
        validatorChain.AddValidator(new DateValidator());
      }
      else if (meetField.RawText.Contains("email", StringComparison.OrdinalIgnoreCase))
      {
        validatorChain.AddValidator(new EmailValidator());
      }
      else if (meetField.RawText.Contains("grade", StringComparison.OrdinalIgnoreCase))
      {
        validatorChain.AddValidator(new GradeValidator());
      }

      return validatorChain;
    }
  }
}

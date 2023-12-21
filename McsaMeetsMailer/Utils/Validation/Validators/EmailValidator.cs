using System;
using System.Net.Mail;

namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class EmailValidator : IValidator
  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool Validate(in string input)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        IsValid = true;
        return true;
      }

      try
      {
        new MailAddress(input);
      }
      catch (FormatException)
      {
        ErrorMessage = $"Email address format is incorrect for \"{input}\".";
        return false;
      }

      IsValid = true;

      return true;
    }
  }
}

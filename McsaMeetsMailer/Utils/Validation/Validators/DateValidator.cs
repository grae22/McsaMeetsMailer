using System;

namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class DateValidator : IValidator
  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool Validate(in string input)
    {
      IsValid = DateTime.TryParse(input, out DateTime result);

      if (!IsValid)
      {
        ErrorMessage = $"Date \"{input}\" is invalid.";
      }

      return IsValid;
    }
  }
}

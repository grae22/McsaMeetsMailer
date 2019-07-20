using System;

namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class DateValidator : IValidator
  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool Validate(in string input)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        SetValid();
        return true;
      }

      bool isValid = DateTime.TryParse(input, out DateTime result);

      if (!isValid)
      {
        SetInvalid($"Date or time \"{input}\" is invalid.");

        return false;
      }

      SetValid();

      return true;
    }

    private void SetValid()
    {
      IsValid = true;
      ErrorMessage = string.Empty;
    }

    private void SetInvalid(in string errorMessage)
    {
      IsValid = false;
      ErrorMessage = errorMessage;
    }
  }
}

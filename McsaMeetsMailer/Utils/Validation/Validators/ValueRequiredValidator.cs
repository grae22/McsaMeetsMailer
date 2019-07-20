namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class ValueRequiredValidator : IValidator

  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool Validate(in string input)
    {
      IsValid = !string.IsNullOrWhiteSpace(input);

      if (!IsValid)
      {
        ErrorMessage = "Value is required.";
      }

      return IsValid;
    }
  }
}

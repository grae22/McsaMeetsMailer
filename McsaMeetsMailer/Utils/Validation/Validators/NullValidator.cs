namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class NullValidator : IValidator, IValidationResults
  {
    public bool IsValid => true;
    public string ErrorMessage => string.Empty;

    public bool Validate(in string input)
    {
      return true;
    }
  }
}

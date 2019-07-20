namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public interface IValidator : IValidationResults
  {
    bool Validate(in string input);
  }
}
namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public interface IValidationResults
  {
    bool IsValid { get; }
    string ErrorMessage { get; }
  }
}
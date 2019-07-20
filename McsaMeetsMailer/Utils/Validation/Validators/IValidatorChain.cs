namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public interface IValidatorChain : IValidationResults
  {
    void AddValidator(in IValidator validator);
    void Validate(in string input);
  }
}

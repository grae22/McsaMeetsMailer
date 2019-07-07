using System;

using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetFieldValue
  {
    public MeetField Field { get; }
    public string Value { get; }
    public IValidationResults ValidationResults => _validator;

    private readonly IValidator _validator;

    public MeetFieldValue(
      in MeetField field,
      in string value,
      in IValidator validator)
    {
      Field = field;
      Value = value;

      _validator = validator ?? throw new ArgumentNullException(nameof(validator));
      _validator.Validate(value);
    }
  }
}

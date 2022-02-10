using System;

using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetFieldValue
  {
    public MeetField Field { get; }
    public string Value { get; }
    public string FormattedValue { get; }
    public IValidationResults ValidationResults => _validatorChain;
    public DateTime? ValueAsDate => GetValueAsDate();

    private readonly IValidatorChain _validatorChain;

    public MeetFieldValue(
      in MeetField field,
      in string value,
      in IValidatorChain validatorChain)
    {
      Field = field;
      Value = value;

      FormattedValue = Field.ValueFormatter.Format(Value);

      _validatorChain = validatorChain ?? throw new ArgumentNullException(nameof(validatorChain));
      _validatorChain.Validate(value);
    }

    private DateTime? GetValueAsDate()
    {
      if (DateTime.TryParse(Value, out DateTime result))
      {
        return result;
      }

      return DateTimeExtensions.ValueIndicatingDateStillToBeAnnounced;
    }
  }
}

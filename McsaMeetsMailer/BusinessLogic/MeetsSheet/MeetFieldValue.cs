﻿using System;

using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetFieldValue
  {
    public MeetField Field { get; }
    public string Value { get; }
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

      _validatorChain = validatorChain ?? throw new ArgumentNullException(nameof(validatorChain));
      _validatorChain.Validate(value);
    }

    private DateTime? GetValueAsDate()
    {
      if (DateTime.TryParse(Value, out DateTime result))
      {
        return result;
      }

      return null;
    }
  }
}

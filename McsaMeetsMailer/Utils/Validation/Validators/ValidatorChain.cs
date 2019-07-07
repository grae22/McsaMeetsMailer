using System;
using System.Collections.Generic;
using System.Linq;

namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class ValidatorChain : IValidationResults
  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private readonly List<IValidator> _validators = new List<IValidator>();

    public void AddValidator(IValidator validator)
    {
      if (validator == null)
      {
        throw new ArgumentNullException(nameof(validator));
      }

      _validators.Add(validator);
    }

    public void Validate(in string input)
    {
      if (!_validators.Any())
      {
        SetValid();
        return;
      }

      foreach (var validator in _validators)
      {
        validator.Validate(input);

        if (validator.IsValid)
        {
          continue;
        }

        SetInvalid(validator.ErrorMessage);

        return;
      }

      SetValid();
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

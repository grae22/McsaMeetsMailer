using System.Collections.Generic;
using System.Linq;

namespace McsaMeetsMailer.Utils.Validation.Validators
{
  public class GradeValidator : IValidator
  {
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; }

    private static readonly int[] ValidGrades = { 1, 2, 3, 4, 5 };

    public bool Validate(in string input)
    {
      IEnumerable<string> potentialGrades = ExtractAllPotentialGrades(input);

      IsValid = AreAllGradesValid(potentialGrades);

      if (!IsValid)
      {
        ErrorMessage = "Unrecognised grade(s) or format incorrect.";
      }

      return IsValid;
    }

    private static IEnumerable<string> ExtractAllPotentialGrades(in string input)
    {
      char[] separators = { ' ',  ',', '&' };

      var potentialGrades = input
        .Split(separators)
        .ToList();

      potentialGrades.RemoveAll(g => g.Length == 0);

      return potentialGrades;
    }

    private static bool AreAllGradesValid(in IEnumerable<string> grades)
    {
      foreach (var grade in grades)
      {
        if (!int.TryParse(grade, out int gradeAsInt))
        {
          return false;
        }

        if (!ValidGrades.Contains(gradeAsInt))
        {
          return false;
        }
      }

      return true;
    }
  }
}

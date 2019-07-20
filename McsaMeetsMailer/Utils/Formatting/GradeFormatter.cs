using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace McsaMeetsMailer.Utils.Formatting
{
  public class GradeFormatter : IFormatter
  {
    private readonly Dictionary<string, string> _friendlyTextByGrade = new Dictionary<string, string>
    {
      { "1", "Family Friendly" },
      { "2", "Easy Hike" },
      { "3", "Serious Hike" },
      { "4", "Very Serious Hike" },
      { "5", "Rock Climbing" }
    };

    public string Format(in string input)
    {
      IEnumerable<string> grades = ExtractAllGrades(input);

      var output = new StringBuilder();
      bool isFirstGrade = true;

      foreach (var grade in grades)
      {
        if (!isFirstGrade)
        {
          output.Append(", ");
        }

        output.Append(grade);

        if (_friendlyTextByGrade.ContainsKey(grade))
        {
          output.Append(" (");
          output.Append(_friendlyTextByGrade[grade]);
          output.Append(')');
        }

        isFirstGrade = false;
      }

      if (output.Length == 0)
      {
        output.Append(input);
      }

      return output.ToString();
    }

    private static IEnumerable<string> ExtractAllGrades(in string input)
    {
      List<string> potentialGrades = input
        .Split(' ', ',', '&')
        .ToList();

      var grades = new List<string>();

      foreach (var potentialGrade in potentialGrades)
      {
        if (!int.TryParse(potentialGrade, out int result))
        {
          continue;
        }

        string grade = result.ToString();

        if (grades.Contains(grade))
        {
          continue;
        }

        grades.Add(grade);
      }

      return grades;
    }
  }
}

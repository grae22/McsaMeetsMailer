using System.Collections.Generic;

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
      string grade = input.Trim();

      if (!_friendlyTextByGrade.ContainsKey(grade))
      {
        return $"{input} (???)";
      }

      return $"{grade} ({_friendlyTextByGrade[grade]})";
    }
  }
}

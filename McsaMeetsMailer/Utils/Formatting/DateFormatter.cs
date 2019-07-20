using System;

namespace McsaMeetsMailer.Utils.Formatting
{
  public class DateFormatter : IFormatter
  {
    private readonly string _format;

    public DateFormatter(in string format)
    {
      _format = format ?? throw new ArgumentNullException(nameof(format));
    }

    public string Format(in string input)
    {
      if (DateTime.TryParse(input, out var dateTime))
      {
        return dateTime.ToString(_format);
      }

      return input;
    }
  }
}

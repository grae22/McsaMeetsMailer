using System;

namespace McsaMeetsMailer.Utils.Formatting
{
  public class PhoneNumberFormatter : IFormatter
  {
    public string Format(
      in string input)
    {
      if (input is null)
      {
        throw new ArgumentNullException(nameof(input));
      }

      string inputWithoutSpaces = input.Replace(" ", string.Empty);

      // Don't know what this is?
      if (inputWithoutSpaces.Length < 9 ||
          inputWithoutSpaces.Length > 12)
      {
        return input;
      }
      // Probably 10 digit number missing the leading zero.
      else if (
        inputWithoutSpaces.Length == 9 &&
        inputWithoutSpaces[0] != '0')
      {
        inputWithoutSpaces = inputWithoutSpaces.Insert(0, "0");
      }
      // Probably 12 digit number missing the leading plus.
      else if (
        inputWithoutSpaces.Length == 11 &&
        inputWithoutSpaces[0] != '+')
      {
        inputWithoutSpaces = inputWithoutSpaces.Insert(0, "+");
      }

      if (inputWithoutSpaces.Length == 10)
      {
        return
          $"{inputWithoutSpaces.Substring(0, 3)} {inputWithoutSpaces.Substring(3, 3)} {inputWithoutSpaces.Substring(6, 4)}";
      }
      else if (
        inputWithoutSpaces.Length == 12 &&
        inputWithoutSpaces[0] == '+')
      {
        return
          $"{inputWithoutSpaces.Substring(0, 3)} {inputWithoutSpaces.Substring(3, 2)} {inputWithoutSpaces.Substring(5, 3)} {inputWithoutSpaces.Substring(8, 4)}";
      }

      return input;
    }
  }
}

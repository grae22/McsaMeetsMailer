using System;
using System.Linq;
using System.Security;

namespace McsaMeetsMailer.Utils.Settings
{
  public class EnvironmentVariableSettings : ISettings
  {
    public string GetString(in string settingName, in string defaultValue)
    {
      string value = Environment.GetEnvironmentVariable(settingName);

      if (string.IsNullOrWhiteSpace(value))
      {
        return defaultValue;
      }

      return value;
    }

    public int GetInteger(in string settingName, in int defaultValue)
    {
      string value = Environment.GetEnvironmentVariable(settingName);

      if (int.TryParse(value, out int valueAsInteger))
      {
        return valueAsInteger;
      }

      return defaultValue;
    }

    public string GetValidString(in string settingName)
    {
      string value = Environment.GetEnvironmentVariable(settingName);

      if (string.IsNullOrWhiteSpace(value))
      {
        throw new InvalidSettingException(settingName, "Setting value cannot be NULL, empty or white-space.");
      }

      return value;
    }

    public int GetValidInteger(in string settingName)
    {
      string value = Environment.GetEnvironmentVariable(settingName);

      if (int.TryParse(value, out int valueAsInteger))
      {
        return valueAsInteger;
      }

      throw new InvalidSettingException(settingName, "Setting value must be a valid integer.");
    }

    public SecureString GetValidSecureString(in string settingName)
    {
      var secureValue = new SecureString();

      GetValidString(settingName)
        .ToCharArray()
        .ToList()
        .ForEach(c => secureValue.AppendChar(c));

      return secureValue;
    }
  }
}

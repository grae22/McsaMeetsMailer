using System;

namespace McsaMeetsMailer.Utils.Settings
{
  public class EnvironmentVariableSettings : ISettings
  {
    public string GetValidValue(in string settingName)
    {
      string value = Environment.GetEnvironmentVariable(settingName);

      if (string.IsNullOrWhiteSpace(value))
      {
        throw new InvalidSettingException(settingName, "Setting value cannot be NULL, empty or white-space.");
      }

      return value;
    }
  }
}

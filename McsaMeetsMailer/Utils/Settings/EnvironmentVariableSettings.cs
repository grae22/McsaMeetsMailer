using System;
using System.Linq;
using System.Security;

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

    public SecureString GetValidSecureValue(in string settingName)
    {
      var secureValue = new SecureString();

      GetValidValue(settingName)
        .ToCharArray()
        .ToList()
        .ForEach(c => secureValue.AppendChar(c));

      return secureValue;
    }
  }
}

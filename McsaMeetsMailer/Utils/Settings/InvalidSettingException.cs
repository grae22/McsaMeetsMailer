using System;

namespace McsaMeetsMailer.Utils.Settings
{
  public class InvalidSettingException : Exception
  {
    public InvalidSettingException(
      in string settingName,
      in string reason)
    :
      base($"The setting \"{settingName}\" is invalid - \"{reason}\".")
    {
    }
  }
}

using System.Security;

namespace McsaMeetsMailer.Utils.Settings
{
  public interface ISettings
  {
    // Returns the specified default value if setting is not found or value is NULL or white-space.
    string GetString(in string settingName, in string defaultValue);

    // Returns the specified default value if setting is not found or value is not an integer.
    int GetInteger(in string settingName, in int defaultValue);

    // Throws InvalidSettingException if setting is not found or value is NULL or white-space.
    string GetValidString(in string settingName);

    // Throws InvalidSettingException if setting is not found or value is not an integer.
    int GetValidInteger(in string settingName);

    // Throws InvalidSettingException if setting is not found or value is NULL or white-space.
    SecureString GetValidSecureString(in string settingName);
  }
}

using System.Security;

namespace McsaMeetsMailer.Utils.Settings
{
  public interface ISettings
  {
    // Throws InvalidSettingException if setting is not found or value is NULL or white-space.
    string GetValidValue(in string settingName);

    // Throws InvalidSettingException if setting is not found or value is NULL or white-space.
    SecureString GetValidSecureValue(in string settingName);
  }
}

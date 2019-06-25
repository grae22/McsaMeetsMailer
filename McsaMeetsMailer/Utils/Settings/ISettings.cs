namespace McsaMeetsMailer.Utils.Settings
{
  public interface ISettings
  {
    // Throws InvalidSettingException if setting is not found or value is NULL or whitespace.
    string GetValidValue(in string settingName);
  }
}

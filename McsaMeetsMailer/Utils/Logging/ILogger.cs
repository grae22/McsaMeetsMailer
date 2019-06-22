namespace McsaMeetsMailer.Utils.Logging
{
  public interface ILogger
  {
    void LogDebug(in string message);
    void LogInfo(in string message);
    void LogWarning(in string message);
    void LogError(in string message);
  }
}

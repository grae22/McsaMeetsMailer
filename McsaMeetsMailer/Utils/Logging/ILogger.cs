using System;

namespace McsaMeetsMailer.Utils.Logging
{
  public interface ILogger
  {
    void LogDebug(in string message);
    void LogInfo(in string message);
    void LogWarning(in string message, in Exception ex = null);
    void LogError(in string message, in Exception ex = null);
  }
}

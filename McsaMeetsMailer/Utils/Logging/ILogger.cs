using System;

namespace McsaMeetsMailer.Utils.Logging
{
  public interface ILogger
  {
    void LogDebug(
      in string message,
      in string sourceClassName);

    void LogInfo(
      in string message,
      in string sourceClassName);

    void LogWarning(
      in string message,
      in string sourceClassName,
      in Exception ex = null);

    void LogError(
      in string message,
      in string sourceClassName,
      in Exception ex = null);
  }
}

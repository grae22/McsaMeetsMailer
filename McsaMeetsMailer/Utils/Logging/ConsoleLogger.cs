using System;
using System.Diagnostics;

namespace McsaMeetsMailer.Utils.Logging
{
  public class ConsoleLogger : ILogger
  {
    public void LogDebug(
      in string message,
      in string sourceClassName)
    {
      Log("Debug", message, sourceClassName);
    }

    public void LogInfo(
      in string message,
      in string sourceClassName)
    {
      Log("Info", message, sourceClassName);
    }

    public void LogWarning(
      in string message,
      in string sourceClassName,
      in Exception ex = null)
    {
      Log("Warning", message, sourceClassName);
    }

    public void LogError(
      in string message,
      in string sourceClassName, 
      in Exception ex = null)
    {
      Log("Error", message, sourceClassName);
    }

    private static void Log(
      in string logType,
      in string message,
      in string sourceClassName)
    {
      Debug.WriteLine($"*** {logType} | {DateTime.Now:yy-MM-dd HH:mm:ss} | {message} | {sourceClassName}");
    }
  }
}

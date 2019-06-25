using System;
using System.Diagnostics;

namespace McsaMeetsMailer.Utils.Logging
{
  public class ConsoleLogger : ILogger
  {
    public void LogDebug(in string message)
    {
      Log("Debug", message);
    }

    public void LogInfo(in string message)
    {
      Log("Info", message);
    }

    public void LogWarning(in string message, in Exception ex = null)
    {
      Log("Warning", message);
    }

    public void LogError(in string message, in Exception ex = null)
    {
      Log("Error", message);
    }

    private void Log(in string logType, in string message)
    {
      Debug.WriteLine($"*** {logType} | {DateTime.Now:yy-MM-dd HH:mm:ss} | {message}");
    }
  }
}

using System;

namespace McsaMeetsMailer.Utils.Logging
{
  public class DummyLogger : ILogger
  {
    public void LogDebug(in string message)
    {
    }

    public void LogInfo(in string message)
    {
    }

    public void LogWarning(in string message, in Exception ex = null)
    {
    }

    public void LogError(in string message, in Exception ex = null)
    {
    }
  }
}

using System;

namespace McsaMeetsMailer.Utils.Extensions
{
  internal static class DateTimeExtensions
  {
    public static DateTime ValueIndicatingDateStillToBeAnnounced { get; } = DateTime.MinValue;

    public static bool IsStillToBeAnnounced(
      this DateTime dateTime)
    {
      return dateTime == ValueIndicatingDateStillToBeAnnounced;
    }
  }
}

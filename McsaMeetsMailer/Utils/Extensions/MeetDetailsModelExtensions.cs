using System;
using System.Linq;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;

namespace McsaMeetsMailer.Utils.Extensions
{
  public static class MeetDetailsModelExtensions
  {
    private const string LeaderFieldName = "Leader";
    private const string DateFieldName = "Date";

    public static MeetFieldValue LeaderField(this MeetDetailsModel details)
    {
      return GetField(
        LeaderFieldName,
        details,
        true);
    }

    public static MeetFieldValue DateField(
      this MeetDetailsModel details,
      in bool raiseExceptionIfNotFound = true)
    {
      return GetField(
        DateFieldName,
        details,
        raiseExceptionIfNotFound);
    }

    private static MeetFieldValue GetField(
      in string fieldName,
      in MeetDetailsModel details,
      in bool raiseExceptionIfNotFound)
    {
      if (details == null)
      {
        throw new ArgumentNullException(nameof(details));
      }

      try
      {
        string fieldNameLocal = fieldName;

        return details
          .FieldValues
          .First(m =>
            m
              .Field
              .FriendlyText
              .Equals(fieldNameLocal, StringComparison.OrdinalIgnoreCase));
      }
      catch (NullReferenceException)
      {
        // Handled below.
      }
      catch (InvalidOperationException)
      {
        // Handled below.
      }

      if (raiseExceptionIfNotFound)
      {
        throw new MissingMeetFieldException(fieldName);
      }

      return null;
    }
  }
}

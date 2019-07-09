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
      return GetField(LeaderFieldName, details);
    }

    public static MeetFieldValue DateField(this MeetDetailsModel details)
    {
      return GetField(DateFieldName, details);
    }

    private static MeetFieldValue GetField(
      in string fieldName,
      in MeetDetailsModel details)
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
        throw new MissingMeetFieldException(fieldName);
      }
      catch (InvalidOperationException)
      {
        throw new MissingMeetFieldException(fieldName);
      }
    }
  }
}

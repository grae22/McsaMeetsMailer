using System;
using System.Linq;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;

namespace McsaMeetsMailer.Utils.Extensions
{
  public static class MeetDetailsModelExtensions
  {
    public static MeetFieldValue LeaderField(this MeetDetailsModel details)
    {
      const string LeaderFieldName = "Leader";

      if (details == null)
      {
        throw new ArgumentNullException(nameof(details));
      }

      try
      {
        return details
          .FieldValues
          .First(m =>
            m
              .Field
              .FriendlyText
              .Equals(LeaderFieldName, StringComparison.OrdinalIgnoreCase));
      }
      catch (NullReferenceException)
      {
        throw new MissingMeetFieldException(LeaderFieldName);
      }
      catch (InvalidOperationException)
      {
        throw new MissingMeetFieldException(LeaderFieldName);
      }
    }
  }
}

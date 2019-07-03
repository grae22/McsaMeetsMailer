using System.Collections.Generic;
using System.Linq;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Validation;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class GoogleSheetToMeetDetailsTransformer
  {
    public static void Process(
      in IMeetsGoogleSheet sheet,
      out IEnumerable<MeetDetailsModel> meetDetailsModels)
    {
      CommonValidation.RaiseExceptionIfArgumentNull(sheet, nameof(sheet));

      var models = new List<MeetDetailsModel>();

      int leaderNameColumnIndex = sheet.FindHeaderIndex(MeetsGoogleSheet.HeaderText_LeaderName, true);
      int leaderEmailColumnIndex = sheet.FindHeaderIndex(MeetsGoogleSheet.HeaderText_LeaderEmail, true);

      foreach (var row in sheet.ValuesByRow)
      {
        var rowAsArray = row.ToArray();

        var newModel = new MeetDetailsModel
        {
          Leader = rowAsArray[leaderNameColumnIndex].Value,
          LeaderEmail = rowAsArray[leaderEmailColumnIndex].Value,
          AdditionalFields = new Dictionary<string, string>()
        };

        int cellIndex = -1;

        foreach (var cellValue in rowAsArray)
        {
          cellIndex++;

          if (cellIndex == leaderNameColumnIndex ||
              cellIndex == leaderEmailColumnIndex)
          {
            continue;
          }

          string header = sheet.Fields.ElementAt(cellIndex).FriendlyText;
          header = header.Replace("#", "");
          header = header.Replace("*", "");

          newModel
            .AdditionalFields
            .Add(header, cellValue.Value);
        }

        models.Add(newModel);
      }

      meetDetailsModels = models;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Extensions;
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

      foreach (var row in sheet.ValuesByRow)
      {
        var newModel = new MeetDetailsModel
        {
          AllFields = sheet.Fields,
          FieldValues = row
        };

        models.Add(newModel);
      }

      meetDetailsModels = models
        .OrderBy(m =>
          m
            .DateField(false)
            ?.ValueAsDate ?? DateTime.MinValue);
    }
  }
}

﻿using System.Collections.Generic;
using System.Linq;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Html;

namespace McsaMeetsMailer.BusinessLogic
{
  public static class FullScheduleEmailBuilder
  {
    public static string Build(IEnumerable<MeetDetailsModel> meetDetails, IHtmlBuilder htmlBuilder)
    {
      List<MeetDetailsModel> meetDetailsList = meetDetails.ToList();

      htmlBuilder.AddStyleSheet("~/css/fullSchedulePreview.css");
      htmlBuilder.AddLineBreak();
      htmlBuilder.AddParagraph("Hi,");
      htmlBuilder.AddParagraph("Please find the full meet schedule below.");
      htmlBuilder.AddLineBreak();

      var requiredHeadings = new List<MeetField>();

      IEnumerable<MeetField> allHeadings = meetDetailsList.FirstOrDefault()?.AllFields.ToList();
      requiredHeadings.AddRange(allHeadings?.Where(x => x.DisplayInHeader));

      var optionalHeadings = new List<MeetField>();

      optionalHeadings.AddRange(allHeadings?.Where(x => !x.DisplayInHeader));

      BuildHeaderTable(
        htmlBuilder,
        requiredHeadings,
        meetDetails);

      htmlBuilder.AddLineBreak();
      htmlBuilder.AddParagraph("Additional meet details:");
      htmlBuilder.AddLineBreak();

      BuildDetailsTable(htmlBuilder, meetDetails);

      return htmlBuilder.GetHtml();
    }

    private static void BuildHeaderTable(
      IHtmlBuilder htmlBuilder,
      IEnumerable<MeetField> headings,
      IEnumerable<MeetDetailsModel> meets)
    {
      htmlBuilder.StartTable();

      htmlBuilder.AddHeadingRow(
        headings
          .OrderBy(x => x.SortOrder)
          .Where(x => x.DisplayInHeader)
          .Select(x => x.FriendlyText));

      foreach(var meet in meets)
      {
        htmlBuilder.AddRow(
          meet
            .FieldValues
            .OrderBy(x => x.Field.SortOrder)
            .Where(x => x.Field.DisplayInHeader)
            .Select(x => x.Value));
      }

      htmlBuilder.EndTable();
    }

    private static void BuildDetailsTable(
      IHtmlBuilder htmlBuilder,
      IEnumerable<MeetDetailsModel> meets)
    {
      foreach (var meet in meets)
      {
        htmlBuilder.AddParagraph(string.Empty);
        htmlBuilder.StartTable();

        List<MeetFieldValue> sortedFields = meet
          .FieldValues
          .OrderBy(x => x.Field.SortOrder)
          .ToList();

        var meetTitleField = sortedFields.First(x => x.Field.IsMeetTitle);

        sortedFields.Remove(meetTitleField);
        sortedFields.Insert(0, meetTitleField);

        foreach (var field in sortedFields)
        {
          if (string.IsNullOrWhiteSpace(field.Value))
          {
            continue;
          }

          string value = field.Value;

          if (!field.ValidationResults.IsValid)
          {
            value = $"INVALID : {value}";
          }

          htmlBuilder.AddRow(
            new[]
            {
              field.Field.FriendlyText,
              value
            });
        }

        htmlBuilder.EndTable();
      }
    }
  }
}
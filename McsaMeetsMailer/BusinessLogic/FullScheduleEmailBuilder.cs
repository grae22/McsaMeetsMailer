using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;

namespace McsaMeetsMailer.BusinessLogic
{
  public static class FullScheduleEmailBuilder
  {
    private const string summaryHeadingStart = "<!--HeaderHeading-->";
    private const string summaryValuesStart = "<!--HeaderValues-->";
    private const string summaryValueStart = "<!--HeaderValue-->";
    private const string summaryLinkValueStart = "<!--HeaderLinkValue-->";
    private const string detailsStart = "<!--Details-->";
    private const string detailStart = "<!--Detail-->";
    private const string endOfHeadingColumn = "</th>";
    private const string endOfColumn = "</td>";
    private const string endOfRow = "</tr>";
    private const string endOfAnchor = "</a>";

    public static string Build(IEnumerable<MeetDetailsModel> meetsDetails)
    {
      string html;

      using (var reader = new StreamReader("Pages/Templates/FullScheduleEmailTemplate.html"))
      {
        html = reader.ReadToEnd();
      }

      var meets = meetsDetails.ToList();

      string headerHeadings = GetHeaderHeadings(html, meets);
      string headerValues = GetHeaderValues(html, meets);
      string details = GetDetails(html, meets);

      html = UpdateHtml(html, summaryHeadingStart, endOfHeadingColumn, headerHeadings);
      html = UpdateHtml(html, summaryValuesStart, endOfRow, headerValues);
      html = UpdateHtml(html, detailsStart, endOfAnchor, details);

      return html;
    }

    private static string GetHeaderHeadings(string html, IEnumerable<MeetDetailsModel> meets)
    {
      IEnumerable<string> sortedHeadings = meets.FirstOrDefault()?
        .FieldValues
        .OrderBy(x => x.Field.SortOrder)
        .Where(x => x.Field.DisplayInHeader)
        .Select(x => x.Field.FriendlyText);

      int startIndex = html.IndexOf(summaryHeadingStart, StringComparison.Ordinal) + summaryHeadingStart.Length;
      int endIndex = html.IndexOf(endOfHeadingColumn, startIndex, StringComparison.Ordinal) + endOfHeadingColumn.Length;
      string headingTemplate = html.Substring(startIndex, endIndex - startIndex);
      var headingsHtml = new StringBuilder("");

      // Headings
      if( sortedHeadings != null )
      {
        foreach( string heading in sortedHeadings )
        {
          headingsHtml.Append( headingTemplate.Replace( "{Heading}", heading ) );
        }
      }

      return headingsHtml.ToString();
    }

    private static string GetHeaderValues(string html, IEnumerable<MeetDetailsModel> meetDetails)
    {
      // Values template
      int summaryValuesStartIndex = html.IndexOf(summaryValuesStart, StringComparison.Ordinal);
      int summaryValuesEndIndex = html.IndexOf(endOfRow, summaryValuesStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string valuesTemplate = html.Substring(summaryValuesStartIndex, summaryValuesEndIndex - summaryValuesStartIndex);

      // Value template
      int valueTemplateStartIndex = valuesTemplate.IndexOf(summaryValueStart, StringComparison.Ordinal) + summaryValueStart.Length;
      int valueTemplateEndIndex = valuesTemplate.IndexOf(endOfColumn, valueTemplateStartIndex, StringComparison.Ordinal) + endOfColumn.Length;
      string valueTemplate = valuesTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      // Anchor template
      int anchorTemplateStartIndex = valuesTemplate.IndexOf(summaryLinkValueStart, StringComparison.Ordinal) + summaryLinkValueStart.Length;
      int anchorTemplateEndIndex = valuesTemplate.IndexOf(endOfColumn, anchorTemplateStartIndex, StringComparison.Ordinal) + endOfColumn.Length;
      string anchorTemplate = valuesTemplate.Substring(anchorTemplateStartIndex, anchorTemplateEndIndex - anchorTemplateStartIndex);

      // Remove templates.
      valuesTemplate = valuesTemplate.Remove(valueTemplateStartIndex, anchorTemplateEndIndex - valueTemplateStartIndex);

      var meetValues = new StringBuilder("");
      var allMeetValues = new List<string>();

      foreach (MeetDetailsModel meetDetail in meetDetails)
      {
        List<MeetFieldValue> sortedFields = meetDetail
          .FieldValues
          .OrderBy(x => x.Field.SortOrder)
          .ToList();

        foreach (MeetFieldValue field in sortedFields)
        {
          string value = field.Value;

          if (!field.ValidationResults.IsValid) value = $"INVALID : {value}";

          if (field.Field.DisplayInHeader)
          {
            if (field.Field.IsMeetTitle)
              meetValues.Append(anchorTemplate.Replace("{Value}", value));
            else
              meetValues.Append(valueTemplate.Replace("{Value}", value));
          }
        }

        allMeetValues.Add(valuesTemplate.Insert(valueTemplateStartIndex, meetValues.ToString()));
        meetValues.Clear();
      }

      return string.Join("", allMeetValues);
    }

    private static string GetDetails(string html, List<MeetDetailsModel> meetDetails)
    {
      // Details table template
      int detailsTableTemplateStartIndex = html.IndexOf(detailsStart, StringComparison.Ordinal) + detailsStart.Length;
      int detailsTableTemplateEndIndex = html.IndexOf(endOfAnchor, detailsTableTemplateStartIndex, StringComparison.Ordinal) + endOfAnchor.Length;
      string detailsTableTemplate = html.Substring(detailsTableTemplateStartIndex, detailsTableTemplateEndIndex - detailsTableTemplateStartIndex);

      // Value template
      int valueTemplateStartIndex = detailsTableTemplate.IndexOf( detailStart, StringComparison.Ordinal) + detailStart.Length;
      int valueTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, valueTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string valueTemplate = detailsTableTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      detailsTableTemplate = detailsTableTemplate.Remove(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      var values = new StringBuilder("");
      var detailsHtml = new StringBuilder();

      foreach (MeetDetailsModel meetDetail in meetDetails)
      {
        List<MeetFieldValue> sortedFields = meetDetail
          .FieldValues
          .OrderBy(x => x.Field.SortOrder)
          .ToList();

        MeetFieldValue meetTitleField = sortedFields.First(x => x.Field.IsMeetTitle);

        sortedFields.Remove(meetTitleField);
        sortedFields.Insert(0, meetTitleField);

        string currentTableTemplate = detailsTableTemplate.Replace("{Anchor}", sortedFields.FirstOrDefault()?.Value);

        foreach (MeetFieldValue field in sortedFields)
        {
          string value = field.Value;

          if (!field.ValidationResults.IsValid) value = $"INVALID : {value}";

          string htmlBlob = valueTemplate.Replace("{Title}", field.Field.FriendlyText);
          htmlBlob = htmlBlob.Replace("{Value}", value);
          values.Append(htmlBlob);
        }

        var index = currentTableTemplate.IndexOf(detailStart, StringComparison.Ordinal);
        detailsHtml = detailsHtml.Append(currentTableTemplate.Insert(index, values.ToString()));
        values = values.Clear();
      }

      return detailsHtml.ToString();
    }

    private static string UpdateHtml(string html, string start, string end, string htmlToInsert)
    {
      int startIndex = html.IndexOf(start, StringComparison.Ordinal);
      int endIndex = html.IndexOf(end, startIndex, StringComparison.Ordinal) + end.Length;
      html = html.Remove(startIndex, endIndex - startIndex);
      html = html.Insert(startIndex, htmlToInsert);

      return html;
    }
  }
}
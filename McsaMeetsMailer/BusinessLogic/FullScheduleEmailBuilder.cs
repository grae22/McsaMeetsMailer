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
    private const string summaryValuesStart_Invalid = "<!--HeaderValues_Invalid-->";
    private const string summaryValueStart = "<!--HeaderValue-->";
    private const string summaryLinkValueStart = "<!--HeaderLinkValue-->";
    private const string detailsStart = "<!--Details-->";
    private const string detailHeaderStart = "<!--DetailHeader-->";
    private const string detailHeaderStart_Invalid = "<!--DetailHeader_Invalid-->";
    private const string detailStart = "<!--Detail-->";
    private const string detailStart_Invalid = "<!--Detail_Invalid-->";
    private const string endOfHeadingColumn = "</th>";
    private const string endOfColumn = "</td>";
    private const string endOfRow = "</tr>";
    private const string endOfAnchor = "</a>";

    public static string Build(IEnumerable<MeetDetailsModel> meetsDetails,
                               string customMessage = "Hi, <br><br>Please find the full meet schedule below.", 
                               bool previewMode = true)
    {
      string html;

      using (var reader = new StreamReader("Pages/Templates/FullScheduleEmailTemplate.html"))
      {
        html = reader.ReadToEnd();
      }

      var meets = meetsDetails.ToList();

      string headerHeadings = GetHeaderHeadings(html, meets);
      string headerValues = GetHeaderValues(html, meets, previewMode);
      string details = GetDetails(html, meets, previewMode);

      html = html.Replace("{CustomMessage}", customMessage);

      html = UpdateHtml(html, summaryHeadingStart, endOfHeadingColumn, headerHeadings);

      // Insert summary content
      int startIndex = html.IndexOf(summaryValueStart, StringComparison.Ordinal);
      int startIndex_Invalid = html.IndexOf(summaryValuesStart_Invalid, StringComparison.Ordinal);
      int endIndex = html.IndexOf(endOfRow, startIndex_Invalid, StringComparison.Ordinal) + endOfRow.Length;
      html = html.Remove(startIndex, endIndex - startIndex);
      html = html.Insert(startIndex, headerValues);

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

    private static string GetHeaderValues(string html, IEnumerable<MeetDetailsModel> meetDetails, bool previewMode)
    {
      // Valid values template
      int summaryValuesStartIndex_Valid = html.IndexOf(summaryValuesStart, StringComparison.Ordinal);
      int summaryValuesEndIndex_Valid = html.IndexOf(endOfRow, summaryValuesStartIndex_Valid, StringComparison.Ordinal) + endOfRow.Length;
      string valuesTemplate_Valid = html.Substring(summaryValuesStartIndex_Valid, summaryValuesEndIndex_Valid - summaryValuesStartIndex_Valid);

      // Valid value template
      int valueTemplateStartIndex_Valid = valuesTemplate_Valid.IndexOf(summaryValueStart, StringComparison.Ordinal) + summaryValueStart.Length;
      int valueTemplateEndIndex_Valid = valuesTemplate_Valid.IndexOf(endOfColumn, valueTemplateStartIndex_Valid, StringComparison.Ordinal) + endOfColumn.Length;
      string valueTemplate_Valid = valuesTemplate_Valid.Substring(valueTemplateStartIndex_Valid, valueTemplateEndIndex_Valid - valueTemplateStartIndex_Valid);

      // Valid anchor template
      int anchorTemplateStartIndex_Valid = valuesTemplate_Valid.IndexOf(summaryLinkValueStart, StringComparison.Ordinal) + summaryLinkValueStart.Length;
      int anchorTemplateEndIndex_Valid = valuesTemplate_Valid.IndexOf(endOfColumn, anchorTemplateStartIndex_Valid, StringComparison.Ordinal) + endOfColumn.Length;
      string anchorTemplate_Valid = valuesTemplate_Valid.Substring(anchorTemplateStartIndex_Valid, anchorTemplateEndIndex_Valid - anchorTemplateStartIndex_Valid);

      // Invalid values template
      int summaryValuesStartIndex_Invalid = html.IndexOf(summaryValuesStart_Invalid, StringComparison.Ordinal);
      int summaryValuesEndIndex_Invalid = html.IndexOf(endOfRow, summaryValuesStartIndex_Invalid, StringComparison.Ordinal) + endOfRow.Length;
      string valuesTemplate_Invalid = html.Substring(summaryValuesStartIndex_Invalid, summaryValuesEndIndex_Invalid - summaryValuesStartIndex_Invalid);

      // Invalid value template
      int valueTemplateStartIndex_Invalid = valuesTemplate_Invalid.IndexOf(summaryValueStart, StringComparison.Ordinal) + summaryValueStart.Length;
      int valueTemplateEndIndex_Invalid = valuesTemplate_Invalid.IndexOf(endOfColumn, valueTemplateStartIndex_Invalid, StringComparison.Ordinal) + endOfColumn.Length;
      string valueTemplate_Invalid = valuesTemplate_Invalid.Substring(valueTemplateStartIndex_Invalid, valueTemplateEndIndex_Invalid - valueTemplateStartIndex_Invalid);

      // Invalid anchor template
      int anchorTemplateStartIndex_Invalid = valuesTemplate_Invalid.IndexOf(summaryLinkValueStart, StringComparison.Ordinal) + summaryLinkValueStart.Length;
      int anchorTemplateEndIndex_Invalid = valuesTemplate_Invalid.IndexOf(endOfColumn, anchorTemplateStartIndex_Invalid, StringComparison.Ordinal) + endOfColumn.Length;
      string anchorTemplate_Invalid = valuesTemplate_Invalid.Substring(anchorTemplateStartIndex_Invalid, anchorTemplateEndIndex_Invalid - anchorTemplateStartIndex_Invalid);

      // Remove templates.
      valuesTemplate_Valid = valuesTemplate_Valid.Remove(valueTemplateStartIndex_Valid, anchorTemplateEndIndex_Valid - valueTemplateStartIndex_Valid);
      valuesTemplate_Invalid = valuesTemplate_Invalid.Remove(valueTemplateStartIndex_Invalid, anchorTemplateEndIndex_Invalid - valueTemplateStartIndex_Invalid);

      var meetValues = new StringBuilder("");
      var allMeetValues = new List<string>();

      foreach (MeetDetailsModel meetDetail in meetDetails)
      {
        List<MeetFieldValue> sortedFields = meetDetail
          .FieldValues
          .OrderBy(x => x.Field.SortOrder)
          .ToList();

        bool invalid = previewMode && meetDetail.FieldValues.Any(x => !x.ValidationResults.IsValid);

        foreach (MeetFieldValue field in sortedFields)
        {
          if (!field.Field.DisplayInHeader)
          {
            continue;
          }

          if (field.Field.IsMeetTitle)
          {
            if (!invalid)
            {
              meetValues.Append(anchorTemplate_Valid.Replace("{Value}", field.Value));
            }
            else
            {
              meetValues.Append(anchorTemplate_Invalid.Replace("{Value}", field.Value));
            }
          }
          else
          {
            if (!invalid)
            {
              meetValues.Append(valueTemplate_Valid.Replace("{Value}", field.Value));
            }
            else
            {
              meetValues.Append(valueTemplate_Invalid.Replace("{Value}", field.Value));
            }
          }
        }

        if (!invalid)
        {
          allMeetValues.Add(valuesTemplate_Valid.Insert(valueTemplateStartIndex_Valid, meetValues.ToString()));
        }
        else
        {
          allMeetValues.Add(valuesTemplate_Invalid.Insert(valueTemplateStartIndex_Invalid, meetValues.ToString()));
        }
        
        meetValues.Clear();
      }

      return string.Join("", allMeetValues);
    }

    private static string GetDetails(string html, List<MeetDetailsModel> meetDetails, bool previewMode)
    {
      // Details table template
      int detailsTableTemplateStartIndex = html.IndexOf(detailsStart, StringComparison.Ordinal) + detailsStart.Length;
      int detailsTableTemplateEndIndex = html.IndexOf(endOfAnchor, detailsTableTemplateStartIndex, StringComparison.Ordinal) + endOfAnchor.Length;
      string detailsTableTemplate = html.Substring(detailsTableTemplateStartIndex, detailsTableTemplateEndIndex - detailsTableTemplateStartIndex);

      // Header template
      int valueHeaderTemplateStartIndex = detailsTableTemplate.IndexOf(detailHeaderStart, StringComparison.Ordinal) + detailHeaderStart.Length;
      int valueHeaderTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, valueHeaderTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string valueHeaderTemplate = detailsTableTemplate.Substring(valueHeaderTemplateStartIndex, valueHeaderTemplateEndIndex - valueHeaderTemplateStartIndex);

      // Invalid header template
      int invalidValueHeaderTemplateStartIndex = detailsTableTemplate.IndexOf(detailHeaderStart_Invalid, StringComparison.Ordinal) + detailHeaderStart_Invalid.Length;
      int invalidValueHeaderTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, invalidValueHeaderTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string invalidValueHeaderTemplate = detailsTableTemplate.Substring(invalidValueHeaderTemplateStartIndex, invalidValueHeaderTemplateEndIndex - invalidValueHeaderTemplateStartIndex);

      // Value template
      int valueTemplateStartIndex = detailsTableTemplate.IndexOf( detailStart, StringComparison.Ordinal) + detailStart.Length;
      int valueTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, valueTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string valueTemplate = detailsTableTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      // Invalid value template
      int invalidValueTemplateStartIndex = detailsTableTemplate.IndexOf(detailStart_Invalid, StringComparison.Ordinal) + detailStart_Invalid.Length;
      int invalidValueTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, invalidValueTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;
      string invalidValueTemplate = detailsTableTemplate.Substring(invalidValueTemplateStartIndex, invalidValueTemplateEndIndex - invalidValueTemplateStartIndex);

      detailsTableTemplate = detailsTableTemplate.Remove(valueHeaderTemplateStartIndex, invalidValueTemplateEndIndex - valueHeaderTemplateStartIndex);

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
          string value = field.ValidationResults.IsValid ? field.FormattedValue : $"INVALID : {field.Value}";
          string htmlBlob;

          bool invalid = previewMode && !field.ValidationResults.IsValid;

          if (field.Field.IsMeetTitle)
          {
            htmlBlob = !invalid ? 
              valueHeaderTemplate.Replace("{Title}", field.Field.FriendlyText) : 
              invalidValueHeaderTemplate.Replace("{Title}", field.Field.FriendlyText);
          }
          else
          {
            htmlBlob = !invalid ? 
              valueTemplate.Replace("{Title}", field.Field.FriendlyText) : 
              invalidValueTemplate.Replace("{Title}", field.Field.FriendlyText);
          }

          htmlBlob = htmlBlob.Replace("{Value}", value);
          values.Append(htmlBlob);
        }

        var index = currentTableTemplate.IndexOf(detailHeaderStart, StringComparison.Ordinal);
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
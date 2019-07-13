using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace McsaMeetsMailer.BusinessLogic
{
  public static class FullScheduleEmailBuilder
  {
    private const string headerHeadingStart =  "<!--HeaderHeading-->";
    private const string headerValuesStart = "<!--HeaderValues-->";
    private const string headerValueStart = "<!--HeaderValue-->";
    private const string detailsStart = "<!--Details-->";
    private const string endOfHeadingColumn =  "</th>";
    private const string endOfColumn =  "</td>";
    private const string endOfRow =  "</tr>";

    public static string Build(IEnumerable<MeetDetailsModel> meetDetails)
    {
      string html;

      using (var reader = new StreamReader("Pages/Templates/FullScheduleEmailTemplate.html"))
      {
        html = reader.ReadToEnd();
      }

      var meetDetailsList = meetDetails.ToList();

      var sortedHeaderHeadings = meetDetailsList.FirstOrDefault()?
                                                .FieldValues
                                                .OrderBy(x => x.Field.SortOrder)
                                                .Where(x => x.Field.DisplayInHeader)
                                                .Select(x => x.Field.FriendlyText);

      IEnumerable<string> sortedHeadings = meetDetailsList.FirstOrDefault()?
                                                          .FieldValues
                                                          .OrderBy(x => x.Field.SortOrder)
                                                          .Where(x => !x.Field.DisplayInHeader)
                                                          .Select(x => x.Field.FriendlyText);

      var headerHeadings = GetHeaderHeadings(html, sortedHeaderHeadings, headerHeadingStart, endOfHeadingColumn);
      var headerValues = GetHeaderValues(html, meetDetailsList, headerValuesStart, endOfRow, headerValueStart, endOfColumn);
      var details = GetDetails(html, meetDetailsList, detailsStart, "</table>");

      html = UpdateHtml(html, headerHeadingStart, endOfHeadingColumn, headerHeadings);
      html = UpdateHtml(html, headerValuesStart, endOfRow, headerValues);
      html = UpdateHtml(html, detailsStart, "</a>", details);

      return html;
    }

    private static string GetHeaderHeadings(string html, IEnumerable<string> headings, string start, string end)
    {
      var startIndex = html.IndexOf(start, StringComparison.Ordinal) + start.Length;
      var endIndex = html.IndexOf(end, startIndex, StringComparison.Ordinal) + end.Length;
      var headingTemplate = html.Substring(startIndex, endIndex - startIndex);
      var headingsHtml = new StringBuilder("");

      // Headings
      if (headings != null)
      {
        foreach (var heading in headings)
        {
          headingsHtml.Append(headingTemplate.Replace("{Heading}", heading));
        }
      }

      return headingsHtml.ToString();
    }

    private static string GetHeaderValues(string html,
                                          List<MeetDetailsModel> meetDetails,
                                          string sectionStart,
                                          string sectionEnd,
                                          string start,
                                          string end)
    {
      int sectionStartIndex = html.IndexOf(sectionStart, StringComparison.Ordinal);
      int sectionEndIndex = html.IndexOf(sectionEnd, sectionStartIndex, StringComparison.Ordinal) + sectionEnd.Length;

      var valuesTemplate = html.Substring(sectionStartIndex, sectionEndIndex - sectionStartIndex);

      var valueTemplateStartIndex = valuesTemplate.IndexOf(start, StringComparison.Ordinal) + start.Length;
      var valueTemplateEndIndex = valuesTemplate.IndexOf(end, valueTemplateStartIndex, StringComparison.Ordinal) + end.Length;

      var valueTemplate = valuesTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      var anchorTemplateStartIndex = valuesTemplate.IndexOf( "<!--Link Value-->", StringComparison.Ordinal ) + "<!--Link Value-->".Length;
      var anchorTemplateEndIndex = valuesTemplate.IndexOf( "</td>", anchorTemplateStartIndex, StringComparison.Ordinal ) + "</td>".Length;

      var anchorTemplate = valuesTemplate.Substring( anchorTemplateStartIndex, anchorTemplateEndIndex - anchorTemplateStartIndex );

      valuesTemplate = valuesTemplate.Remove(valueTemplateStartIndex, anchorTemplateEndIndex - valueTemplateStartIndex);

      var values = new StringBuilder("");
      var valueArray = new List<string>();

      foreach (MeetDetailsModel meetDetail in meetDetails)
      {
        List<MeetFieldValue> sortedFields = meetDetail.FieldValues
                                                      .OrderBy(x => x.Field.SortOrder)
                                                      .ToList();

        foreach (MeetFieldValue field in sortedFields)
        {
          string value = field.Value;

          if (!field.ValidationResults.IsValid)
          {
            value = $"INVALID : {value}";
          }

          if (field.Field.DisplayInHeader)
          {
            if (field.Field.IsMeetTitle)
            {
              values.Append(anchorTemplate.Replace("{Value}", value));
            }
            else
            {
              values.Append( valueTemplate.Replace( "{Value}", value ) );
            }
          }
        }

        valueArray.Add(valuesTemplate.Insert(valueTemplateStartIndex, values.ToString()));
        values.Clear();
      }

      return string.Join("", valueArray);
    }

    private static string GetDetails(string html,
                                    List<MeetDetailsModel> meetDetails,
                                    string start,
                                    string end)
    {
      var detailsTableTemplateStartIndex = html.IndexOf( start, StringComparison.Ordinal ) + start.Length;
      var detailsTableTemplateEndIndex = html.IndexOf( "</a>", detailsTableTemplateStartIndex, StringComparison.Ordinal ) + "</a>".Length;

      var detailsTableTemplate = html.Substring( detailsTableTemplateStartIndex, detailsTableTemplateEndIndex - detailsTableTemplateStartIndex );

      var valueTemplateStartIndex = detailsTableTemplate.IndexOf("<!--Detail-->", StringComparison.Ordinal) + "<!--Detail-->".Length;
      var valueTemplateEndIndex = detailsTableTemplate.IndexOf(endOfRow, valueTemplateStartIndex, StringComparison.Ordinal) + endOfRow.Length;

      var valueTemplate = detailsTableTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);
      detailsTableTemplate =
        detailsTableTemplate.Remove(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);
      var values = new StringBuilder("");
      var detailsHtml = new StringBuilder();

      foreach (MeetDetailsModel meetDetail in meetDetails)
      {
        List<MeetFieldValue> sortedFields = meetDetail.FieldValues
                                                      .OrderBy(x => x.Field.SortOrder)
                                                      .ToList();

        var meetTitleField = sortedFields.First(x => x.Field.IsMeetTitle);

        sortedFields.Remove(meetTitleField);
        sortedFields.Insert(0, meetTitleField);

        var currentTableTemplate = detailsTableTemplate.Replace("{Anchor}", sortedFields.FirstOrDefault()?.Value);

        foreach (MeetFieldValue field in sortedFields)
        {
          string value = field.Value;

          if (!field.ValidationResults.IsValid)
          {
            value = $"INVALID : {value}";
          }

          var htmlBlob = valueTemplate.Replace("{Title}", field.Field.FriendlyText);
          htmlBlob = htmlBlob.Replace("{Value}", value);
          values.Append(htmlBlob);
        }

        detailsHtml = detailsHtml.Append( currentTableTemplate.Insert(currentTableTemplate.IndexOf( "<!--Detail-->", StringComparison.Ordinal ), values.ToString()));
        values = values.Clear();
      }

      return detailsHtml.ToString();
    }

    private static string UpdateHtml(string html, string start, string end, string htmlToInsert)
    {
      var startIndex = html.IndexOf(start, StringComparison.Ordinal);
      var endIndex = html.IndexOf(end, startIndex, StringComparison.Ordinal) + end.Length;
      html = html.Remove(startIndex, endIndex - startIndex);
      html = html.Insert(startIndex, htmlToInsert);

      return html;
    }

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
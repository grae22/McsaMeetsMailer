using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Html;

namespace McsaMeetsMailer.BusinessLogic
{
  public static class FullScheduleEmailBuilder
  {
    private const string headerHeadingStart =  "<!--HeaderHeading-->";
    private const string headerHeadingEnd =  "</th>";

    public static string Build(IEnumerable<MeetDetailsModel> meetDetails)
    {
      string html;

      using (var reader = new StreamReader("Pages/Templates/FullScheduleEmailTemplate.html"))
      {
        html = reader.ReadToEnd();
      }

      var meetDetailsList = meetDetails.ToList();

      var headerHeadings = GetHeadings(html,
                                       meetDetailsList.FirstOrDefault()?.FieldValues.Where(x => x.Field.DisplayInHeader).ToList(),
                                       headerHeadingStart,
                                       headerHeadingEnd);

      var headerValues = GetValues(html, meetDetailsList, "<!--HeaderValues-->", "</tr>", "<!--HeaderValue-->", "</td>", true);

      string headings = GetHeadings(html,
                                    meetDetailsList.FirstOrDefault()?.FieldValues.Where(x => !x.Field.DisplayInHeader).ToList(),
                                    "<!--Heading-->",
                                    "</th>");

      var values = GetValues(html, meetDetailsList, "<!--Values-->", "</tr>", "<!--Value-->", "</td>", false);

      var headerHeadingStartIndex = html.IndexOf(headerHeadingStart, StringComparison.Ordinal) + headerHeadingStart.Length;
      var headerHeadingEndIndex = html.IndexOf(headerHeadingEnd, headerHeadingStartIndex, StringComparison.Ordinal) + headerHeadingEnd.Length;
      html = html.Remove(headerHeadingStartIndex, headerHeadingEndIndex - headerHeadingStartIndex);
      html = html.Insert(headerHeadingStartIndex, headerHeadings);

      var headingStartIndex = html.IndexOf("<!--Heading-->", StringComparison.Ordinal) + "<!--Heading-->".Length;
      var headingEndIndex = html.IndexOf("</th>", headingStartIndex, StringComparison.Ordinal) + "</th>".Length;
      html = html.Remove(headingStartIndex, headingEndIndex - headingStartIndex);
      html = html.Insert(headingStartIndex, headings);

      var headerValuesStartIndex = html.IndexOf("<!--HeaderValues-->", StringComparison.Ordinal);
      var headerValuesEndIndex = html.IndexOf("</tr>", headerValuesStartIndex, StringComparison.Ordinal) + "</tr>".Length;
      html = html.Remove(headerValuesStartIndex, headerValuesEndIndex - headerValuesStartIndex);
      html = html.Insert(headerValuesStartIndex, headerValues);

      var valuesStartIndex = html.IndexOf("<!--Values-->", StringComparison.Ordinal);
      var valuesEndIndex = html.IndexOf("</tr>", valuesStartIndex, StringComparison.Ordinal) + "</tr>".Length;
      html = html.Remove(valuesStartIndex, valuesEndIndex - valuesStartIndex);
      html = html.Insert(valuesStartIndex, values);

      return html;
    }

    private static string GetHeadings(string html, List<MeetFieldValue> meetFieldValues, string start, string end)
    {
      var startIndex = html.IndexOf(start, StringComparison.Ordinal) + start.Length;
      var endIndex = html.IndexOf(end, startIndex, StringComparison.Ordinal) + end.Length;
      var headingTemplate = html.Substring(startIndex, endIndex - startIndex);
      var headings = new StringBuilder("");

      // Headings
      if (meetFieldValues != null)
      {
        foreach (var field in meetFieldValues)
        {
          headings.Append(headingTemplate.Replace("{Heading}", field.Field.FriendlyText));
        }
      }

      return headings.ToString();
    }

    private static string GetValues(string html, 
                                    List<MeetDetailsModel> meetDetails, 
                                    string sectionStart,
                                    string sectionEnd, 
                                    string start, 
                                    string end, 
                                    bool isHeader)
    {
      var sectionStartIndex = html.IndexOf(sectionStart, StringComparison.Ordinal);
      var sectionEndIndex = html.IndexOf(sectionEnd, sectionStartIndex, StringComparison.Ordinal) + sectionEnd.Length;

      var valuesTemplate = html.Substring(sectionStartIndex, sectionEndIndex - sectionStartIndex);

      var valueTemplateStartIndex = valuesTemplate.IndexOf(start, StringComparison.Ordinal) + start.Length;
      var valueTemplateEndIndex = valuesTemplate.IndexOf(end, valueTemplateStartIndex, StringComparison.Ordinal) + end.Length;

      var valueTemplate = valuesTemplate.Substring(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);
      valuesTemplate = valuesTemplate.Remove(valueTemplateStartIndex, valueTemplateEndIndex - valueTemplateStartIndex);

      var values = new StringBuilder("");
      var valueArray = new List<string>();

      foreach (var meetDetail in meetDetails)
      {
        foreach (var field in meetDetail.FieldValues)
        {
          if (isHeader && field.Field.DisplayInHeader)
          {
            values.Append(valueTemplate.Replace("{Value}", field.Value));
          }
          else if (!isHeader && !field.Field.DisplayInHeader)
          {
            values.Append(valueTemplate.Replace("{Value}", field.Value));
          }
        }

        valueArray.Add(valuesTemplate.Insert(valueTemplateStartIndex, values.ToString()));
        values.Clear();
      }

      return string.Join("", valueArray);
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
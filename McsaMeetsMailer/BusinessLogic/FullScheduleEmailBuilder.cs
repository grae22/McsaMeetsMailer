using System.Collections.Generic;
using System.Linq;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Html;

namespace McsaMeetsMailer.BusinessLogic
{
  public static class FullScheduleEmailBuilder
  {
    private struct MeetDetailsInfo
    {
      public List<string> Values { get; set; }
    }

    public static string Build(IEnumerable<MeetDetailsModel> meetDetails, IHtmlBuilder htmlBuilder)
    {
      List<MeetDetailsModel> meetDetailsList = meetDetails.ToList();

      htmlBuilder.AddStyleSheet("~/css/fullSchedulePreview.css");
      htmlBuilder.AddLineBreak();
      htmlBuilder.AddParagraph("Hi,");
      htmlBuilder.AddParagraph("Please find the full meet schedule below.");
      htmlBuilder.AddLineBreak();

      var requiredHeadings = new List<string>
      {
        "No.",
        "Leader",
        "Leader Email"
      };

      Dictionary<string, string>.KeyCollection allHeadings = meetDetailsList.FirstOrDefault()?.AdditionalFields.Keys;
      requiredHeadings.AddRange(allHeadings?.Where(x => x.Contains("#")));

      var optionalHeadings = new List<string>
      {
        "No."
      };

      optionalHeadings.AddRange(allHeadings?.Where(x => !x.Contains("#")));

      var requiredMeetDetailsInfos = new List<MeetDetailsInfo>();
      var optionalMeetDetailsInfos = new List<MeetDetailsInfo>();
      

      for (var i = 0; i < meetDetailsList.Count(); ++i)
      {
        var meetDetail = meetDetailsList[i];

        var requiredMeetDetailsInfo = new MeetDetailsInfo
        {
          Values = new List<string> {(i + 1).ToString(), meetDetail.Leader, meetDetail.LeaderEmail}
        };
        var optionalMeetDetailsInfo = new MeetDetailsInfo {Values = new List<string> { (i + 1).ToString() } };

        foreach (var field in meetDetail.AdditionalFields)
        {
          if (field.Key.Contains("#"))
          {
            requiredMeetDetailsInfo.Values.Add(field.Value);
          }
          else
          {
            optionalMeetDetailsInfo.Values.Add(field.Value);
          }
        }

        requiredMeetDetailsInfos.Add(requiredMeetDetailsInfo);
        optionalMeetDetailsInfos.Add(optionalMeetDetailsInfo);
      }

      BuildTable(htmlBuilder, requiredHeadings, requiredMeetDetailsInfos);
      htmlBuilder.AddLineBreak();
      htmlBuilder.AddParagraph("Additional meet details:");
      htmlBuilder.AddLineBreak();
      BuildTable(htmlBuilder, optionalHeadings, optionalMeetDetailsInfos);

      return htmlBuilder.GetHtml();
    }

    private static void BuildTable(IHtmlBuilder htmlBuilder, List<string> headings, List<MeetDetailsInfo> meetDetailsInfos)
    {
      htmlBuilder.StartTable();

      for (var i = 0; i < headings.Count; ++i)
      {
        headings[i] = headings[i].Replace("*", "");
        headings[i] = headings[i].Replace("#", "");
      }

      htmlBuilder.AddHeadingRow(headings);

      foreach(var meetDetailsInfo in meetDetailsInfos)
      {
        htmlBuilder.AddRow(meetDetailsInfo.Values);
      }

      htmlBuilder.EndTable();
    }
  }
}
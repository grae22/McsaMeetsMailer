using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetsModel : PageModel
  {
    public string Html { get; private set; }

    private readonly IMeetsService _meetsService;
    private readonly IHostingEnvironment _hostingEnvironment;

    public MeetsModel(
      IMeetsService meetsService,
      IHostingEnvironment hostingEnvironment)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
      _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
    }

    public async Task OnGet()
    {
      IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets(DateTime.Now);

      if (meets == null)
      {
        return;
      }

      bool printerFriendly = Request.Query.ContainsKey("printerFriendly");

      if (printerFriendly)
      {
        Html = BuildPrinterFriendlyHtml(meets);
      }
      else
      {
        Html = FullScheduleEmailBuilder.Build(
          meetsDetails: meets,
          templatesPath: $@"{_hostingEnvironment.WebRootPath}\templates",
          previewMode: false,
          printerFriendly: printerFriendly);
      }
    }

    private static string BuildPrinterFriendlyHtml(
      in IEnumerable<MeetDetailsModel> meets)
    {
      if (!meets.Any())
      {
        return string.Empty;
      }

      var allMeetValuesByColumn = new List<List<string>>();

      // Add headers.
      var headers = new List<string>();
      allMeetValuesByColumn.Add(headers);

      meets
        .First()
        .AllFields
        .ToList()
        .ForEach(x => headers.Add(x.FriendlyText));

      // Compile a list of headers for which no meet has a value.
      var headersToSkip = new List<string>();

      foreach (var header in headers)
      {
        bool headerHasValues =
          meets
            .Where(m =>
              m
                .FieldValues
                .FirstOrDefault(f => f.Field.FriendlyText == header)
                ?.FormattedValue
                ?.Any() ?? false)
            .Any();

        if (headerHasValues)
        {
          continue;
        }

        headersToSkip.Add(header);
      }

      headersToSkip
        .ForEach(h => headers.Remove(h));

      // Get all meets' values.
      foreach (var meet in meets)
      {
        var currentMeetValues = new List<string>();
        allMeetValuesByColumn.Add(currentMeetValues);

        // Loop through each field (current meet may not have value for this field).
        foreach (var field in meet.AllFields)
        {
          if (headersToSkip.Contains(field.FriendlyText))
          {
            continue;
          }

          // Try get this meet's value for the current field (may not have one).
          MeetFieldValue value = meet
            .FieldValues
            .FirstOrDefault(m =>
              m.Field.RawText == field.RawText);

          string valueAsString = value?.FormattedValue ?? string.Empty;

          currentMeetValues.Add(valueAsString);
        }
      }

      var htmlBuilder = new StringBuilder();

      htmlBuilder.Append("<b style=\"align: center;\">MCSA-KZN Meets</b><table style=\"font-size: small; border-collapse: collapse;\">");

      var isFirstRow = true;

      foreach (var row in allMeetValuesByColumn)
      {
        htmlBuilder.Append("<tr>");

        row
          .ForEach(
            value =>
            {
              string wrapStyle = value.Length > 20 ? "wrap" : "nowrap";
              string fontStyle = isFirstRow ? "bold" : string.Empty;

              htmlBuilder.Append($"<td style=\"border: 1px solid black; white-space: {wrapStyle}; font-weight: {fontStyle};\">");
              htmlBuilder.Append(value);
              htmlBuilder.Append("</td>");
            });

        htmlBuilder.Append("</tr>");

        isFirstRow = false;
      }

      htmlBuilder.Append("</table>");

      return htmlBuilder.ToString();
    }
  }
}
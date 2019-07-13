using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Html;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class FullSchedulePreviewModel : PageModel
  {
    public string html;

    private readonly IMeetsService _meetsService;

    public FullSchedulePreviewModel(IMeetsService meetsService)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
    }

    public async Task OnGet()
    {
      IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();

      if (meets == null)
      {
        return;
      }

      if (Request.Query.ContainsKey("leader"))
      {
        html = FullScheduleEmailBuilder
          .Build(
            meets
              .Where(m =>
                m
                  .LeaderField()
                  .Value
                  .Equals(Request.Query["leader"], StringComparison.OrdinalIgnoreCase)),
            new HtmlBuilder());
      }
      else
      {
        html = FullScheduleEmailBuilder.Build(meets, new HtmlBuilder());
      }
    }
  }
}
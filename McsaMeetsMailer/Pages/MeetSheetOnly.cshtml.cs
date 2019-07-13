using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetSheetOnlyModel : PageModel
  {
    public string Html { get; private set; }

    private readonly IMeetsService _meetsService;

    public MeetSheetOnlyModel(IMeetsService meetsService)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
    }

    public async Task OnGet()
    {
      IEnumerable<MeetDetailsModel> meets;

      if (Request.Query.ContainsKey("leader"))
      {
        meets = await _meetsService.RetrieveMeets(Request.Query["leader"]);
      }
      else
      {
        meets = await _meetsService.RetrieveMeets();
      }

      if (meets == null)
      {
        return;
      }

      Html = FullScheduleEmailBuilder.Build(meets);
    }
  }
}
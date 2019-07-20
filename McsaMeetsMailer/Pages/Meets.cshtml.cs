using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetsModel : PageModel
  {
    public string Html { get; private set; }

    private readonly IMeetsService _meetsService;

    public MeetsModel(IMeetsService meetsService)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
    }

    public async Task OnGet()
    {
      IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets(DateTime.Now);

      if (meets == null)
      {
        return;
      }

      Html = FullScheduleEmailBuilder.Build(
        meets,
        string.Empty,
        false);
    }
  }
}
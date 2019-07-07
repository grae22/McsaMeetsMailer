using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;
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

      html = FullScheduleEmailBuilder.Build( meets, new HtmlBuilder() );
    }
  }
}
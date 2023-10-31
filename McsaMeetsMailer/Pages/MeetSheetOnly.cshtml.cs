using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetSheetOnlyModel : PageModel
  {
    public string Html { get; private set; }

    private readonly IMeetsService _meetsService;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public MeetSheetOnlyModel(
      IMeetsService meetsService,
      IWebHostEnvironment hostingEnvironment)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
      _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
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
        meets = await _meetsService.RetrieveMeets(DateTime.Now);
      }

      if (meets == null)
      {
        return;
      }

      Html = FullScheduleEmailBuilder.Build(
        meets,
        $@"{_hostingEnvironment.WebRootPath}\templates");
    }
  }
}
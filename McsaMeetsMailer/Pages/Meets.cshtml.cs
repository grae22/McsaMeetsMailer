using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
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

      Html = FullScheduleEmailBuilder.Build(
        meetsDetails: meets,
        templatesPath: $@"{_hostingEnvironment.WebRootPath}\templates",
        previewMode: false);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class FullSchedulePreviewModel : PageModel
  {
    public string html;

    private readonly IMeetsService _meetsService;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public FullSchedulePreviewModel(
      IMeetsService meetsService,
      IWebHostEnvironment hostingEnvironment)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
      _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
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
            $@"{_hostingEnvironment.WebRootPath}\templates");
      }
      else
      {
        html = FullScheduleEmailBuilder.Build(
          meets,
          $@"{_hostingEnvironment.WebRootPath}\templates");
      }
    }
  }
}
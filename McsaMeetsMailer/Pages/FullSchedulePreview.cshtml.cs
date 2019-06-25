using System;
using System.Collections.Generic;
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
#if false
      var collection = new List<MeetDetailsModel>
      {
        new MeetDetailsModel()
        {
          Leader = "Graeme",
          LeaderEmail = "grae22@gmail.com",
          AdditionalFields = new Dictionary<string, string>()
          {
            {"StartDate", new DateTime( 2019, 8, 5, 8, 0, 0 ).Date.ToString()},
            {"EndDate", new DateTime( 2019, 8, 5, 5, 0, 0 ).Date.ToString()}
          }
        },
      };

      html = FullScheduleEmailBuilder.Build(collection, new HtmlBuilder());
#else
      IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();

      if (meets == null)
      {
        return;
      }

      html = FullScheduleEmailBuilder.Build(meets, new HtmlBuilder());
#endif
    }
  }
}
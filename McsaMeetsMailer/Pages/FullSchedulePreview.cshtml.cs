using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using McsaMeetsMailer.Utils.Html;

namespace McsaMeetsMailer.Pages
{
  public class FullSchedulePreviewModel : PageModel
  {
    public string html;

    public void OnGet()
    {
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

      html = FullScheduleEmailBuilder.Build( collection, new HtmlBuilder() );
    }
  }
}
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class SendEmailModel : PageModel
  {
    public string DebugOptionsVisibility { get; private set; }

    public void OnGet()
    {
      DebugOptionsVisibility = Request.Query.ContainsKey("debug") ? "visible" : "hidden";
    }
  }
}
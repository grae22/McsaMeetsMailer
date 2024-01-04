using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class SendEmailModel : PageModel
  {
    public bool OptionsEnabled { get; private set; }
    public string DebugOptionsVisibility { get; private set; }

    public void OnGet()
    {
      OptionsEnabled = Request.Query.ContainsKey("enabled");
      DebugOptionsVisibility = Request.Query.ContainsKey("debug") ? "visible" : "hidden";
    }
  }
}
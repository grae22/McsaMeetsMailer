using System;

using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetSheetEmailPreviewModel : PageModel
  {
    private readonly IMeetsService _meetsService;
    private readonly IEmailSenderService _emailSenderService;

    public MeetSheetEmailPreviewModel(
      IMeetsService meetsService,
      IEmailSenderService emailSenderService)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
      _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
    }

    public void OnGet()
    {
    }

    public void SendEmailToAddress(in string address)
    {
      //IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();
    }
  }
}
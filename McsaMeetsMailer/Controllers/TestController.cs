using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Html;

using Microsoft.AspNetCore.Mvc;

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {
    private readonly IEmailAddressService _emailAddressService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMeetsService _meetsService;

    public TestController(
      IEmailAddressService emailAddressService,
      IEmailSenderService emailSenderService,
      IMeetsService meetsService)
    {
      _emailAddressService = emailAddressService ?? throw new ArgumentNullException(nameof(emailAddressService));
      _emailSenderService = emailSenderService;
      _meetsService = meetsService;
    }

    [Route("retrieveEmailAddresses")]
    public async Task<ActionResult> RetrieveEmailAddresses()
    {
      try
      {
        await _emailAddressService.RetrieveEmailAddresses();
      }
      catch (Exception)
      {
        return StatusCode(500);
      }

      return Ok();
    }

    [Route( "sendEmail" )]
    public async Task<ActionResult> SendEmail()
    {
      try
      {
        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();
        string html = FullScheduleEmailBuilder.Build(meets, new HtmlBuilder());
        var addressBook = await _emailAddressService.RetrieveEmailAddresses();

        _emailSenderService.Send(html, addressBook.FullScheduleEmailAddresses);
      }
      catch (Exception e)
      {
        return StatusCode(500);
      }

      return Ok();
    }
  }
}
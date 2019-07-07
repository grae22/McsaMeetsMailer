using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Html;
using McsaMeetsMailer.Utils.Logging;

using Microsoft.AspNetCore.Mvc;

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {
    private static readonly string ClassName = $"[{typeof(TestController).Name}]";

    private readonly ILogger _logger;
    private readonly IEmailAddressService _emailAddressService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMeetsService _meetsService;

    public TestController(
      ILogger logger,
      IEmailAddressService emailAddressService,
      IEmailSenderService emailSenderService,
      IMeetsService meetsService)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _emailAddressService = emailAddressService ?? throw new ArgumentNullException(nameof(emailAddressService));
      _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
    }

    [Route("retrieveEmailAddresses")]
    public async Task<ActionResult> RetrieveEmailAddresses()
    {
      try
      {
        await _emailAddressService.RetrieveEmailAddresses();
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while retrieving email addresses.", ClassName, ex);
        return StatusCode(500);
      }

      return Ok();
    }

    [Route( "sendFullScheduleEmail" )]
    public async Task<ActionResult> SendFullScheduleEmail()
    {
      try
      {
        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();
        string html = FullScheduleEmailBuilder.Build(meets, new HtmlBuilder());
        var addressBook = await _emailAddressService.RetrieveEmailAddresses();

        _emailSenderService.Send(html, addressBook.FullScheduleEmailAddresses);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email.", ClassName, ex);
        return StatusCode(500);
      }

      return Ok();
    }
  }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private static readonly string ClassName = $"[{typeof(TestController).Name}]";

    private readonly ILogger _logger;
    private readonly IEmailAddressService _emailAddressService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMeetsService _meetsService;

    public EmailController(
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
    
    [Route("sendFullScheduleToAddress")]
    public async Task<ActionResult> SendFullScheduleToAddress()
    {
      try
      {
        string emailAddress;

        using (var reader = new StreamReader(Request.Body))
        {
          emailAddress = reader.ReadToEnd();
        }

        string[] emailAddresses = emailAddress.Split(';');

        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();
        string html = FullScheduleEmailBuilder.Build(meets);

        _logger.LogInfo($"Sending full schedule email to address \"{emailAddresses.Join(";")}\"...", ClassName);

        _emailSenderService.Send(html, emailAddresses);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to address.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }

      return Ok();
    }
    
    [Route("sendFullScheduleToAll")]
    public async Task<ActionResult> SendFullScheduleToAll()
    {
      try
      {
        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();
        string html = FullScheduleEmailBuilder.Build(meets);

        IEmailAddresses addresses = await _emailAddressService.RetrieveEmailAddresses();

        _logger.LogInfo("Sending full schedule email to all...", ClassName);

        _emailSenderService.Send(html, addresses.FullScheduleEmailAddresses);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to all.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }

      return Ok();
    }
  }
}
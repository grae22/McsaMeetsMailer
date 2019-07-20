using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.Settings;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private static readonly string ClassName = $"[{typeof(TestController).Name}]";

    private const string SettingName_MeetsPageUrl = "MCSA-KZN_Meets_MeetsPageUrl";

    private readonly ILogger _logger;
    private readonly IEmailAddressService _emailAddressService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMeetsService _meetsService;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly string _meetsPageUrl;

    public EmailController(
      ISettings settings,
      ILogger logger,
      IEmailAddressService emailAddressService,
      IEmailSenderService emailSenderService,
      IMeetsService meetsService,
      IHostingEnvironment hostingEnvironment)
    {
      if (settings == null)
      {
        throw new ArgumentNullException(nameof(settings));
      }

      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _emailAddressService = emailAddressService ?? throw new ArgumentNullException(nameof(emailAddressService));
      _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
      _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));

      _meetsPageUrl = settings.GetValidString(SettingName_MeetsPageUrl);
    }

    [Route("sendFullScheduleToAddress")]
    public async Task<ActionResult> SendFullScheduleToAddress(EmailContent emailContent)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(emailContent.Address))
        {
          return BadRequest("Address cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(emailContent.Subject))
        {
          return BadRequest("Subject cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(emailContent.Body))
        {
          return BadRequest("Email body cannot be empty");
        }

        string[] emailAddresses = emailContent.Address.Split(';');

        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets();

        string emailBody = FullScheduleEmailBuilder.Build(
          meets,
          $@"{_hostingEnvironment.WebRootPath}\templates",
          emailContent.Body,
          _meetsPageUrl,
          false);

        _logger.LogInfo($"Sending full schedule email to address \"{emailAddresses.Join(";")}\"...", ClassName);

        _emailSenderService.Send(
          emailContent.Subject,
          emailBody,
          emailAddresses);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to address.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }

      return new JsonResult(Ok());
    }

    [Route("sendFullScheduleToAll")]
    public async Task<ActionResult> SendFullScheduleToAll(EmailContent emailContent)
    {
      try
      {
        IEmailAddresses addresses = await _emailAddressService.RetrieveEmailAddresses();

        _logger.LogInfo("Sending full schedule email to all...", ClassName);

        emailContent.Address = addresses
          .FullScheduleEmailAddresses
          .Join(";");

        return await SendFullScheduleToAddress(emailContent);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to all.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }
    }
  }
}
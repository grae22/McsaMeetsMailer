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

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private const string ClassName = nameof(EmailController);

    private const string SettingName_MeetsPageUrl = "MCSA-KZN_Meets_MeetsPageUrl";

    private readonly ILogger _logger;
    private readonly IEmailAddressService _emailAddressService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMeetsService _meetsService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly string _meetsPageUrl;

    public EmailController(
      ISettings settings,
      ILogger logger,
      IEmailAddressService emailAddressService,
      IEmailSenderService emailSenderService,
      IMeetsService meetsService,
      IWebHostEnvironment hostingEnvironment)
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

    [Route("sendToAddress")]
    public async Task<ActionResult> SendToAddress(EmailContent emailContent)
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

        if (string.IsNullOrWhiteSpace(emailContent.SendMeetsUpUntilThisDate) ||
            !DateTime.TryParse(
              emailContent.SendMeetsUpUntilThisDate,
              out DateTime latestDate))
        {
          return BadRequest($"Meets date range is invalid: \"{emailContent.SendMeetsUpUntilThisDate}\".");
        }

        string[] emailAddresses = emailContent.Address.Split(';');

        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets(
          DateTime.Now,
          latestDate);

        string emailBody = FullScheduleEmailBuilder.Build(
          meets,
          $@"{_hostingEnvironment.WebRootPath}\templates",
          emailContent.Body,
          _meetsPageUrl,
          false);

        _logger.LogInfo($"Sending full schedule email to address \"{string.Join(";", emailAddresses)}\"...", ClassName);

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

    [HttpGet]
    [Route("sendDefaultAbridgedToAddress")]
    public async Task<ActionResult> SendDefaultAbridgedToAddress(string address)
    {
      try
      {
        string[] emailAddresses = address.Split(';');

        IEnumerable<MeetDetailsModel> meets = await _meetsService.RetrieveMeets(
          DateTime.Now,
          DateTime.Now.AddMonths(1));

        string emailBody = FullScheduleEmailBuilder.Build(
          meets,
          $@"{_hostingEnvironment.WebRootPath}\templates",
          EmailConstants.DefaultBodyAbridged,
          _meetsPageUrl,
          false);

        _logger.LogInfo($"Sending full schedule email to address \"{string.Join(";", emailAddresses)}\"...", ClassName);

        _emailSenderService.Send(
          EmailConstants.EmailSubjectAbridged,
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

    [Route("sendToAll")]
    public async Task<ActionResult> SendToAll(EmailContent emailContent)
    {
      try
      {
        IEmailAddresses addresses = await _emailAddressService.RetrieveEmailAddresses();

        _logger.LogInfo("Sending full schedule email to all...", ClassName);

        emailContent.Address = string.Join(";", addresses.FullScheduleEmailAddresses);

        return await SendToAddress(emailContent);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to all.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }
    }

    [Route("sendDefaultAbridgedToAll")]
    public async Task<ActionResult> SendDefaultAbdridgedToAll()
    {
      try
      {
        IEmailAddresses emailAddresses = await _emailAddressService.RetrieveEmailAddresses();

        _logger.LogInfo("Sending full schedule email to all...", ClassName);

        string addresses = string.Join(";", emailAddresses.FullScheduleEmailAddresses);

        return await SendDefaultAbridgedToAddress(addresses);
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception while sending full schedule email to all.", ClassName, ex);
        return StatusCode(500, ex.Message);
      }
    }
  }
}
using System;
using System.Threading.Tasks;

using McsaMeetsMailer.Services;

using Microsoft.AspNetCore.Mvc;

namespace McsaMeetsMailer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {
    private readonly IEmailAddressService _emailAddressService;

    public TestController(IEmailAddressService emailAddressService)
    {
      _emailAddressService = emailAddressService ?? throw new ArgumentNullException(nameof(emailAddressService));
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
  }
}
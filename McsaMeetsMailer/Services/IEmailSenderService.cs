using System.Collections.Generic;

namespace McsaMeetsMailer.Services
{
  public interface IEmailSenderService
  {
    void Send(
      in string subject,
      in string htmlContent,
      in IEnumerable<string> emailAddresses);
  }
}

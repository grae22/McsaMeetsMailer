using System.Collections.Generic;

namespace McsaMeetsMailer.Services
{
  public interface IEmailSenderService
  {
    void Send(string htmlContent, IEnumerable<string> emailAddresses);
  }
}

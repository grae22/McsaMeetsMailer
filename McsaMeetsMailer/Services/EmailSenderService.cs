using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

using McsaMeetsMailer.Utils.Settings;

namespace McsaMeetsMailer.Services
{
  public class EmailSenderService : IEmailSenderService
  {
    private readonly SmtpClient _client;
    private readonly string _emailAddress;
    private readonly string _password;

    private const string SettingName_EmailAddress = "MCSA-KZN_Meets_EmailAddress";
    private const string SettingName_Password = "MCSA-KZN_Meets_EmailAddressPassword";

    public EmailSenderService(ISettings settings)
    {
      _client = new SmtpClient("smtp.gmail.com", 587);
      _emailAddress = settings.GetValidValue(SettingName_EmailAddress);
      _password = settings.GetValidValue(SettingName_Password);
    }

    public void Send(string htmlContent, IEnumerable<string> toEmailAddresses)
    {
      _client.Credentials = new NetworkCredential(_emailAddress, _password);
      _client.EnableSsl = true;

      using (var message = new MailMessage())
      {
        message.From = new MailAddress(_emailAddress);

        foreach (string toEmailAddress in toEmailAddresses)
        {
          message.To.Add(new MailAddress(toEmailAddress));
        }

        message.Subject = "MCSA Full Schedule";
        message.Body = htmlContent;
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;

        _client.Send(message);
      }
    }
  }
}
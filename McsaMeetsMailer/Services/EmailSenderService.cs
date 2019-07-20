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
    private readonly string _emailAddressDisplayName;
    private readonly string _password;

    private const string SettingName_EmailAddress = "MCSA-KZN_Meets_EmailAddress";
    private const string SettingName_EmailAddressDisplayName = "MCSA-KZN_Meets_EmailAddressDisplayName";
    private const string SettingName_Password = "MCSA-KZN_Meets_EmailAddressPassword";

    public EmailSenderService(ISettings settings)
    {
      string smtpHost = settings.GetString("MCSA-KZN_Meets_SmtpHost", "smtp.gmail.com");
      int smtpPort = settings.GetInteger("MCSA-KZN_Meets_SmtpPort", 587);

      _client = new SmtpClient(smtpHost, smtpPort);
      _emailAddress = settings.GetValidString(SettingName_EmailAddress);
      _emailAddressDisplayName = settings.GetValidString(SettingName_EmailAddressDisplayName);
      _password = settings.GetValidString(SettingName_Password);
    }

    public void Send(
      in string subject,
      in string htmlContent,
      in IEnumerable<string> toEmailAddresses)
    {
      _client.Credentials = new NetworkCredential(_emailAddress, _password);
      _client.EnableSsl = true;

      using (var message = new MailMessage())
      {
        message.From = new MailAddress(_emailAddress, _emailAddressDisplayName);

        foreach (string toEmailAddress in toEmailAddresses)
        {
          message.To.Add(new MailAddress(toEmailAddress));
        }

        message.Subject = subject;
        message.Body = htmlContent;
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;

        _client.Send(message);
      }
    }
  }
}
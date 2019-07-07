using System;

using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public class EmailAddressGoogleSheetFactory : IEmailAddressGoogleSheetFactory
  {
    public IEmailAddressGoogleSheet CreateSheet(
      in Uri googleSheetUri,
      in IRestRequestMaker requestMaker,
      in ILogger logger)
    {
      return new EmailAddressGoogleSheet(
        googleSheetUri,
        requestMaker,
        logger);
    }
  }
}

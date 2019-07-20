using System;

using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public interface IEmailAddressGoogleSheetFactory
  {
    IEmailAddressGoogleSheet CreateSheet(
      in Uri googleSheetUri,
      in IRestRequestMaker requestMaker,
      in ILogger logger);
  }
}

using System;

using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic
{
  public interface IMeetsGoogleSheetFactory
  {
    IMeetsGoogleSheet CreateSheet(
      in Uri googleSheetUri,
      in IRestRequestMaker requestMaker,
      in ILogger logger);
  }
}

using System;

using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetsGoogleSheetFactory : IMeetsGoogleSheetFactory
  {
    public IMeetsGoogleSheet CreateSheet(
      in Uri googleSheetUri,
      in IRestRequestMaker requestMaker,
      in ILogger logger)
    {
      return new MeetsGoogleSheet(
        googleSheetUri,
        requestMaker,
        logger);
    }
  }
}

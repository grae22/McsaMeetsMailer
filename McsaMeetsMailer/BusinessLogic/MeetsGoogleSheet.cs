using System;
using System.Threading.Tasks;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Validation;

namespace McsaMeetsMailer.BusinessLogic
{
  public class MeetsGoogleSheet
  {
    public static async Task<MeetsGoogleSheet> Retrieve(
      Uri googleSheetUrl,
      IRestRequestMaker requestMaker,
      ILogger logger)
    {
      CommonValidation.RaiseExceptionIfArgumentNull(googleSheetUrl, nameof(googleSheetUrl));
      CommonValidation.RaiseExceptionIfArgumentNull(requestMaker, nameof(requestMaker));
      CommonValidation.RaiseExceptionIfArgumentNull(logger, nameof(logger));

      try
      {
        logger.LogDebug($"Retrieving google-sheet \"{googleSheetUrl.AbsolutePath}\"...");

        var sheet = await requestMaker.Get<GoogleSheet>(googleSheetUrl);

        // TODO: Extract data from sheet.

        return new MeetsGoogleSheet();
      }
      catch (RestRequestException ex)
      {
        logger.LogError($"Failed to retrieve google-sheet \"{googleSheetUrl.AbsolutePath}\", an exception occurred.", ex);
        return null;
      }

      return null;
    }
  }
}

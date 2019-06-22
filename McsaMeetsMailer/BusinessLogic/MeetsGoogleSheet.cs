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
    private const string FirstCellText = "# Date";

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

        if (sheet == null)
        {
          throw new RestRequestException("Null sheet returned", null);
        }

        FindFirstCellCoordinates(
          sheet,
          out int row,
          out int column);

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

    private static void FindFirstCellCoordinates(
      in GoogleSheet sheet,
      out int row,
      out int column)
    {
      if (sheet.values == null)
      {
        throw new MeetsGoogleSheetFormatException("Sheet 'values' field cannot be null.");
      }

      for (row = 0; row < sheet.values.Length; row++)
      {
        for (column = 0; column < sheet.values[row].Length; column++)
        {
          if (sheet.values[row][column].Equals(FirstCellText, StringComparison.OrdinalIgnoreCase))
          {
            return;
          }
        }
      }

      throw new MeetsGoogleSheetFormatException($"First cell not found (\"{FirstCellText}\").");
    }
  }
}

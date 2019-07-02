using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public class EmailAddressGoogleSheet
  {
    public IEnumerable<IEnumerable<string>> DataByRow => _dataByRow;

    private static readonly string ClassName = typeof(EmailAddressGoogleSheet).Name;

    private enum ColumnIndices
    {
      FullSchedule
    }

    private static string[] ColumnHeaders =
    {
      "Full Schedule"
    };

    private readonly Uri _googleSheetUri;
    private readonly IRestRequestMaker _requestMaker;
    private readonly ILogger _logger;
    private List<List<string>> _dataByRow = new List<List<string>>();

    public EmailAddressGoogleSheet(
      Uri googleSheetUri,
      IRestRequestMaker requestMaker,
      ILogger logger)
    {
      _googleSheetUri = googleSheetUri ?? throw new ArgumentNullException(nameof(googleSheetUri));
      _requestMaker = requestMaker ?? throw new ArgumentNullException(nameof(requestMaker));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Retrieve()
    {
      try
      {
        _logger.LogDebug($"Retrieving email address google-sheet \"{_googleSheetUri.AbsolutePath}\"...", ClassName);

        GoogleSheet sheet = await _requestMaker.Get<GoogleSheet>(_googleSheetUri);

        if (sheet == null)
        {
          _logger.LogError("Null email address google-sheet returned.", ClassName);
          return false;
        }

        ValidateColumnHeaders(sheet);
      }
      catch (RestRequestException ex)
      {
        _logger.LogError(
          $"Failed to retrieve email address google-sheet \"{_googleSheetUri.AbsolutePath}\", an exception occurred.",
          ClassName,
          ex);

        return false;
      }

      return true;
    }

    private static void ValidateColumnHeaders(in GoogleSheet sheet)
    {
      if (sheet.values.Length == 0)
      {
        throw new EmailAddressGoogleSheetFormatException("Sheet has no rows.");
      }

      int expectedColumnCount = Enum.GetValues(typeof(ColumnIndices)).Length;
      int actualColumnCount = sheet.values[0].Length;

      if (actualColumnCount < expectedColumnCount)
      {
        throw new EmailAddressGoogleSheetFormatException($"Sheet has too few columns - expected {expectedColumnCount}, found {actualColumnCount}.");
      }

      foreach (int index in Enum.GetValues(typeof(ColumnIndices)))
      {
        string expectedText = ColumnHeaders[index];
        string text = sheet.values[0][index];

        if (text.Equals(expectedText, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        throw new EmailAddressGoogleSheetFormatException(
          $"Column header \"{expectedText}\" not found in column {index}, found \"{text}\" instead.");
      }
    }
  }
}

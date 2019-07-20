using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public class EmailAddressGoogleSheet : IEmailAddressGoogleSheet, IEmailAddresses
  {
    public IEmailAddresses EmailAddresses => this;
    public IEnumerable<string> FullScheduleEmailAddresses => GetColumnData(ColumnIndices.FullSchedule);

    private enum ColumnIndices
    {
      FullSchedule
    }

    private static readonly string ClassName = typeof(EmailAddressGoogleSheet).Name;

    private static readonly string[] ColumnHeaders =
    {
      "Full Schedule"
    };

    private readonly Uri _googleSheetUri;
    private readonly IRestRequestMaker _requestMaker;
    private readonly ILogger _logger;
    private readonly Dictionary<ColumnIndices, List<string>> _dataByColumnIndex = new Dictionary<ColumnIndices, List<string>>();

    private bool _dataExtractedOk;

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
        ExtractData(sheet);
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

    private void ValidateColumnHeaders(in GoogleSheet sheet)
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

      for (var index = 0; index < expectedColumnCount; index++)
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

      _logger.LogInfo("Email address sheet column headers validated ok.", ClassName);
    }

    private void ExtractData(in GoogleSheet sheet)
    {
      int columnCount = Enum.GetValues(typeof(ColumnIndices)).Length;

      for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
      {
        ReadColumnValues(
          sheet,
          columnIndex,
          out List<string> values);

        _dataByColumnIndex.Add((ColumnIndices)columnIndex, values);
      }

      _dataExtractedOk = true;

      _logger.LogInfo("Email address sheet data extracted ok.", ClassName);
    }

    private void ReadColumnValues(
      in GoogleSheet sheet,
      in int columnIndex,
      out List<string> values)
    {
      _logger.LogDebug($"Reading \"{ColumnHeaders[columnIndex]}\" column values...", ClassName);

      values = new List<string>();

      for (var row = 1; row < sheet.values.Length; row++)
      {
        string value = sheet.values[row][columnIndex];

        if (string.IsNullOrWhiteSpace(value))
        {
          continue;
        }

        values.Add(value);

        _logger.LogDebug(value, ClassName);
      }
    }

    private IEnumerable<string> GetColumnData(in ColumnIndices index)
    {
      if (!_dataExtractedOk)
      {
        return new string[0];
      }

      return _dataByColumnIndex[index];
    }
  }
}

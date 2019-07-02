using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

namespace McsaMeetsMailer.BusinessLogic
{
  public class MeetsGoogleSheet : IMeetsGoogleSheet
  {
    public const string HeaderText_Date = "# Date*";
    public const string HeaderText_LeaderName = "# Leader*";
    public const string HeaderText_LeaderEmail = "Leader Email*";

    public IEnumerable<string> Headers => _headers;
    public IEnumerable<IEnumerable<string>> DataByRow => _dataByRow;

    private static readonly string ClassName = typeof(MeetsGoogleSheet).Name;

    private const string FirstCellText = HeaderText_Date;

    private readonly Uri _googleSheetUri;
    private readonly IRestRequestMaker _requestMaker;
    private readonly ILogger _logger;
    private List<string> _headers = new List<string>();
    private List<List<string>> _dataByRow = new List<List<string>>();

    public MeetsGoogleSheet(
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
        _logger.LogDebug($"Retrieving google-sheet \"{_googleSheetUri.AbsolutePath}\"...", ClassName);

        GoogleSheet sheet = await _requestMaker.Get<GoogleSheet>(_googleSheetUri);

        if (sheet == null)
        {
          _logger.LogError($"Null sheet returned.", ClassName);
          return false;
        }

        FindFirstCellCoordinates(
          sheet,
          out int headerRowIndex,
          out int headerColumnIndex);

        FindColumnCount(
          sheet,
          headerRowIndex,
          headerColumnIndex,
          out int headerColumnCount);

        FindLastRow(
          sheet,
          out int lastRow);

        ReadHeaders(
          sheet,
          headerRowIndex,
          headerColumnIndex,
          headerColumnCount,
          out _headers);

        ReadData(
          sheet,
          headerRowIndex,
          headerColumnIndex,
          headerColumnCount,
          lastRow,
          out _dataByRow);
      }
      catch (RestRequestException ex)
      {
        _logger.LogError(
          $"Failed to retrieve google-sheet \"{_googleSheetUri.AbsolutePath}\", an exception occurred.",
          ClassName,
          ex);

        return false;
      }

      return true;
    }

    public int FindHeaderIndex(in string headerText, in bool raiseExceptionIfNotFound = false)
    {
      for (var i = 0; i < Headers.Count(); i++)
      {
        bool isMatch = Headers.ElementAt(i).Equals(headerText, StringComparison.OrdinalIgnoreCase);

        if (isMatch)
        {
          return i;
        }
      }

      if (raiseExceptionIfNotFound)
      {
        throw new MeetsGoogleSheetFormatException($"No header found with text \"{headerText}\".");
      }

      return -1;
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

    private static void FindColumnCount(
      in GoogleSheet sheet,
      in int headerRow,
      in int headerFirstColumn,
      out int columnCount)
    {
      if (sheet.values == null)
      {
        throw new MeetsGoogleSheetFormatException("Sheet 'values' field cannot be null.");
      }

      int column;

      for (column = headerFirstColumn; column < sheet.values[headerRow].Length; column++)
      {
        string cellValue = sheet.values[headerRow][column];

        if (string.IsNullOrWhiteSpace(cellValue))
        {
          break;
        }
      }

      columnCount = column - headerFirstColumn;

      if (columnCount <= 0)
      {
        throw new MeetsGoogleSheetFormatException($"Column count cannot be zero or negative, was {columnCount}.");
      }
    }

    private static void FindLastRow(
      in GoogleSheet sheet,
      out int lastRow)
    {
      if (sheet.values == null)
      {
        throw new MeetsGoogleSheetFormatException("Sheet 'values' field cannot be null.");
      }

      for (lastRow = sheet.values.Length - 1; lastRow >= 0; lastRow--)
      {
        for (var column = 0; column < sheet.values[lastRow].Length; column++)
        {
          string cellValue = sheet.values[lastRow][column];

          if (!string.IsNullOrWhiteSpace(cellValue))
          {
            return;
          }
        }
      }

      throw new MeetsGoogleSheetFormatException($"Last row not found (\"{FirstCellText}\").");
    }

    private static void ReadHeaders(
      in GoogleSheet sheet,
      in int headerRow,
      in int headerColumn,
      in int headerColumnCount,
      out List<string> headers)
    {
      headers = new List<string>();

      for (var column = headerColumn; headers.Count < headerColumnCount; column++)
      {
        headers.Add(sheet.values[headerRow][column]);
      }
    }

    private static void ReadData(
      in GoogleSheet sheet,
      in int headerRow,
      in int headerColumn,
      in int headerColumnCount,
      in int lastRow,
      out List<List<string>> dataByRow)
    {
      dataByRow = new List<List<string>>();

      for (var row = headerRow + 1; row <= lastRow; row++)
      {
        bool rowHasData = false;

        var rowData = new List<string>();

        for (var column = headerColumn; column < headerColumn + headerColumnCount; column++)
        {
          string cellValue = sheet.values[row][column];

          rowData.Add(cellValue);

          if (!rowHasData &&
              !string.IsNullOrWhiteSpace(cellValue))
          {
            rowHasData = true;
          }
        }

        if (rowHasData)
        {
          dataByRow.Add(rowData);
        }
      }
    }
  }
}

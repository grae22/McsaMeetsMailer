using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Formatting;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Validation.Validators;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetsGoogleSheet : IMeetsGoogleSheet
  {
    public IEnumerable<MeetField> Fields => _fields;
    public IEnumerable<IEnumerable<MeetFieldValue>> ValuesByRow => _valuesByRow;

    private static readonly string ClassName = typeof(MeetsGoogleSheet).Name;

    private const string HeaderText_Date = "# Date*";
    private const string HeaderText_MeetTitle = "# Meet Title*";
    private const string FirstCellText = HeaderText_Date;
    private const char HeaderSpecialChar_DisplayInHeader = '#';
    private const char HeaderSpecialChar_Required = '*';

    private readonly Uri _googleSheetUri;
    private readonly IRestRequestMaker _requestMaker;
    private readonly ILogger _logger;
    private List<MeetField> _fields = new List<MeetField>();
    private List<List<MeetFieldValue>> _valuesByRow = new List<List<MeetFieldValue>>();

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
        _logger.LogDebug($"Retrieving meets google-sheet \"{_googleSheetUri.AbsolutePath}\"...", ClassName);

        GoogleSheet sheet = await _requestMaker.Get<GoogleSheet>(_googleSheetUri);

        if (sheet == null)
        {
          _logger.LogError("Null meets google-sheet returned.", ClassName);
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
          ref _fields);

        ReadData(
          sheet,
          headerRowIndex,
          headerColumnIndex,
          headerColumnCount,
          lastRow,
          ref _valuesByRow);
      }
      catch (RestRequestException ex)
      {
        _logger.LogError(
          $"Failed to retrieve meets google-sheet \"{_googleSheetUri.AbsolutePath}\", an exception occurred.",
          ClassName,
          ex);

        return false;
      }

      return true;
    }

    public int FindHeaderIndex(
      in string rawHeaderText,
      in bool raiseExceptionIfNotFound = false)
    {
      for (var i = 0; i < Fields.Count(); i++)
      {
        bool isMatch = Fields
          .ElementAt(i)
          .RawText
          .Equals(rawHeaderText, StringComparison.OrdinalIgnoreCase);

        if (isMatch)
        {
          return i;
        }
      }

      if (raiseExceptionIfNotFound)
      {
        throw new MeetsGoogleSheetFormatException($"No header found with text \"{rawHeaderText}\".");
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

      throw new MeetsGoogleSheetFormatException("Last row not found.");
    }

    private static void ReadHeaders(
      in GoogleSheet sheet,
      in int headerRow,
      in int headerColumn,
      in int headerColumnCount,
      ref List<MeetField> fields)
    {
      fields.Clear();

      for (var column = headerColumn; fields.Count < headerColumnCount; column++)
      {
        string value = sheet.values[headerRow][column].Trim();

        var field = new MeetField(
          IsColumnHeaderForHeaderDisplayField(value),
          IsColumnHeaderForRequiredField(value),
          value,
          StripSpecialCharactersFromColumnHeader(value),
          fields.Count,
          IsColumnHeaderForMeetTitle(value),
          GetFormatterForColumnHeader(value));

        fields.Add(field);
      }
    }

    private void ReadData(
      in GoogleSheet sheet,
      in int headerRow,
      in int headerColumn,
      in int headerColumnCount,
      in int lastRow,
      ref List<List<MeetFieldValue>> dataByRow)
    {
      dataByRow.Clear();

      for (var row = headerRow + 1; row <= lastRow; row++)
      {
        bool rowHasData = false;

        var rowData = new List<MeetFieldValue>();

        for (var column = headerColumn; column < headerColumn + headerColumnCount; column++)
        {
          if (column >= sheet.values[row].Length)
          {
            continue;
          }

          string cellValue = sheet.values[row][column].Trim();

          int fieldIndex = column - headerColumn;

          if (fieldIndex < 0 || fieldIndex >= _fields.Count)
          {
            throw new MeetsGoogleSheetFormatException(
              $"Attempted to access invalid field index {fieldIndex}, max index is {_fields.Count - 1}.");
          }

          MeetField field = _fields[fieldIndex];

          IValidatorChain validatorChain = MeetSheetValueValidatorFactory.CreateValidator(field);

          var newFieldValue = new MeetFieldValue(
            _fields[fieldIndex],
            cellValue,
            validatorChain);

          LogFieldValueValidationErrors(newFieldValue);

          rowData.Add(newFieldValue);

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

    private void LogFieldValueValidationErrors(in MeetFieldValue fieldValue)
    {
      if (fieldValue.ValidationResults.IsValid)
      {
        return;
      }

      _logger.LogDebug(
        $"Validation error on field \"{fieldValue.Field.FriendlyText}\" : \"{fieldValue.ValidationResults.ErrorMessage}\".",
        ClassName);
    }

    private static bool IsColumnHeaderForHeaderDisplayField(in string columnHeaderText)
    {
      return columnHeaderText.Length > 0 &&
             columnHeaderText[0] == HeaderSpecialChar_DisplayInHeader;
    }

    private static bool IsColumnHeaderForRequiredField(in string columnHeaderText)
    {
      return columnHeaderText.Length > 0 &&
             columnHeaderText[columnHeaderText.Length - 1] == HeaderSpecialChar_Required;
    }

    private static bool IsColumnHeaderForMeetTitle(in string columnHeaderText)
    {
      return columnHeaderText.Equals(HeaderText_MeetTitle, StringComparison.OrdinalIgnoreCase);
    }

    private static string StripSpecialCharactersFromColumnHeader(in string columnHeaderText)
    {
      return columnHeaderText
        .Replace($"{HeaderSpecialChar_DisplayInHeader}", "")
        .Replace($"{HeaderSpecialChar_Required}", "")
        .Trim();
    }

    private static IFormatter GetFormatterForColumnHeader(in string columnHeaderText)
    {
      if (columnHeaderText.Contains("date", StringComparison.OrdinalIgnoreCase))
      {
        return new DateFormatter("d MMM (ddd)");
      }

      if (columnHeaderText.Contains("time", StringComparison.OrdinalIgnoreCase))
      {
        return new DateFormatter("HH:mm");
      }

      return NullFormatter.Instance();
    }
  }
}

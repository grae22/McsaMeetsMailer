﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Validation;

namespace McsaMeetsMailer.BusinessLogic
{
  public class MeetsGoogleSheet
  {
    public IEnumerable<string> Headers { get; }
    public IEnumerable<IEnumerable<string>> DataByRow { get; }

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
          out List<string> headers);

        ReadData(
          sheet,
          headerRowIndex,
          headerColumnIndex,
          headerColumnCount,
          lastRow,
          out List<List<string>> dataByRow);

        return new MeetsGoogleSheet(headers, dataByRow);
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

      for (var column = headerColumn; column < headerColumnCount; column++)
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
        bool rowHasData = true;

        var rowData = new List<string>();

        for (var column = headerColumn; column < headerColumnCount; column++)
        {
          string cellValue = sheet.values[row][column];
          
          rowData.Add(cellValue);

          rowHasData &= !string.IsNullOrWhiteSpace(cellValue);
        }

        if (rowHasData)
        {
          dataByRow.Add(rowData);
        }
      }
    }

    public MeetsGoogleSheet(
      IEnumerable<string> headers,
      IEnumerable<IEnumerable<string>> dataByRow)
    {
      Headers = headers ?? throw new ArgumentNullException(nameof(headers));
      DataByRow = dataByRow ?? throw new ArgumentNullException(nameof(dataByRow));
    }
  }
}

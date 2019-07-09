using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services.Exceptions;
using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

namespace McsaMeetsMailer.Services
{
  public class MeetsService : IMeetsService
  {
    private static readonly string ClassName = typeof(MeetsService).Name;

    private const string SettingName_MeetsGoogleSheetId = "MCSA-KZN_Meets_MeetsGoogleSheetId";
    private const string SettingName_GoogleAppKey = "MCSA-KZN_Meets_GoogleAppKey";
    private const string GoogleSheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
    private const string SheetRange = "A1:Z500";

    private readonly string _meetsGoogleSheetId;
    private readonly string _googleAppKey;
    private readonly IRestRequestMaker _requestMaker;
    private readonly IMeetsGoogleSheetFactory _googleSheetFactory;
    private readonly ILogger _logger;

    public MeetsService(
      in ISettings settings,
      in IRestRequestMaker requestMaker,
      in IMeetsGoogleSheetFactory googleSheetFactory,
      in ILogger logger)
    {
      if (settings == null)
      {
        throw new ArgumentNullException(nameof(settings));
      }

      _requestMaker = requestMaker ?? throw new ArgumentNullException(nameof(requestMaker));
      _googleSheetFactory = googleSheetFactory ?? throw new ArgumentNullException(nameof(googleSheetFactory));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _meetsGoogleSheetId = settings.GetValidValue(SettingName_MeetsGoogleSheetId);
      _googleAppKey = settings.GetValidValue(SettingName_GoogleAppKey);
    }

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveAllMeets()
    {
      var url = $"{GoogleSheetsBaseUrl}{_meetsGoogleSheetId}/values/Sheet1!{SheetRange}?key={_googleAppKey}";

      Uri uri;

      try
      {
        uri = new Uri(url);
      }
      catch (UriFormatException ex)
      {
        _logger.LogError($"Failed to build URI from URL \"{url}\".", ClassName, ex);

        throw new MeetsServiceException(
          "Exception while building URI.",
          ex);
      }

      _logger.LogDebug($"Retrieving all meets from \"{url}\"...", ClassName);

      IMeetsGoogleSheet sheet = _googleSheetFactory.CreateSheet(
        uri,
        _requestMaker,
        _logger);

      try
      {
        bool result = await sheet.Retrieve();

        if (result == false)
        {
          _logger.LogError("Failed to retrieve all meets.", ClassName);

          throw new MeetsServiceException("Unknown error while retrieving all meets.");
        }
      }
      catch (MeetsGoogleSheetFormatException ex)
      {
        _logger.LogError("Exception while retrieving all meets.", ClassName, ex);

        throw new MeetsServiceException(
          "Exception while retrieving all meets.",
          ex);
      }

      _logger.LogDebug("Retrieved all meets, transforming into models...", ClassName);

      GoogleSheetToMeetDetailsTransformer.Process(
        sheet,
        out IEnumerable<MeetDetailsModel> meetDetailsModels);

      return meetDetailsModels;
    }

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets(string leaderName)
    {
      var allMeets = await RetrieveAllMeets();

      if (allMeets == null)
      {
        return null;
      }

      try
      {
        return allMeets
          .Where(m =>
            m
              .LeaderField()
              .Value
              .Equals(leaderName, StringComparison.OrdinalIgnoreCase));
      }
      catch (MissingFieldException ex)
      {
        _logger.LogError(
          $"Exception while retrieving meets for leader \"{leaderName}\".",
          ClassName,
          ex);

        throw new MeetsServiceException(
          "Exception while retrieving meets for leader.",
          ex);
      }
    }

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets(DateTime earliestDate)
    {
      var allMeets = await RetrieveAllMeets();

      if (allMeets == null)
      {
        return null;
      }

      try
      {
        return allMeets
          .Where(m =>
            m
              .Date()
              .ValueAsDate
              .Value >= earliestDate);
      }
      catch (MissingFieldException ex)
      {
        _logger.LogError(
          $"Exception while retrieving meets from earliest date \"{earliestDate:f}\".",
          ClassName,
          ex);

        throw new MeetsServiceException(
          "Exception while retrieving meets for earliest date.",
          ex);
      }
    }
  }
}

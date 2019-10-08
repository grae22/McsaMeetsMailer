using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services.Exceptions;
using McsaMeetsMailer.Utils.Cache;
using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

namespace McsaMeetsMailer.Services
{
  public class MeetsService : IMeetsService
  {
    private const string ClassName = nameof(MeetsService);

    private const string SettingName_MeetsGoogleSheetId = "MCSA-KZN_Meets_MeetsGoogleSheetId";
    private const string SettingName_GoogleAppKey = "MCSA-KZN_Meets_GoogleAppKey";
    private const string GoogleSheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
    private const string SheetRange = "A1:Z500";

    private readonly string _meetsGoogleSheetId;
    private readonly string _googleAppKey;
    private readonly IRestRequestMaker _requestMaker;
    private readonly IMeetsGoogleSheetFactory _googleSheetFactory;
    private readonly ILogger _logger;
    private readonly TimeBasedAutoRefresher<IMeetsGoogleSheet> _refreshedMeetSheet;

    public MeetsService(
      in ISettings settings,
      in IRestRequestMaker requestMaker,
      in IMeetsGoogleSheetFactory googleSheetFactory,
      in IDateTimeService dateTimeService,
      in ILogger logger)
    {
      if (settings == null)
      {
        throw new ArgumentNullException(nameof(settings));
      }

      _requestMaker = requestMaker ?? throw new ArgumentNullException(nameof(requestMaker));
      _googleSheetFactory = googleSheetFactory ?? throw new ArgumentNullException(nameof(googleSheetFactory));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _meetsGoogleSheetId = settings.GetValidString(SettingName_MeetsGoogleSheetId);
      _googleAppKey = settings.GetValidString(SettingName_GoogleAppKey);
    }

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets()
    {
      IMeetsGoogleSheet meetSheet = CreateMeetSheet();

      await RetrieveMeetSheet(meetSheet);

      _logger.LogDebug("Retrieved all meets, transforming into models...", ClassName);

      GoogleSheetToMeetDetailsTransformer.Process(
        meetSheet,
        out IEnumerable<MeetDetailsModel> meetDetailsModels);

      return meetDetailsModels;
    }

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets(string leaderName)
    {
      var allMeets = await RetrieveMeets();

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
      var allMeets = await RetrieveMeets();

      if (allMeets == null)
      {
        return null;
      }

      try
      {
        var invalidDate = DateTime.MinValue;

        return allMeets
          .Where(m =>
          {
            var date = DateTime.MinValue;

            if (m.DateField(false) != null)
            {
              date = m
                .DateField()
                .ValueAsDate ?? invalidDate;
            }

            return date >= earliestDate || date == invalidDate;
          });
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

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets(
      DateTime earliestDate,
      DateTime latestDate)
    {
      var allMeets = await RetrieveMeets();

      if (allMeets == null)
      {
        return null;
      }

      try
      {
        return allMeets
          .Where(m =>
          {
            bool dateIsOnOrAfterEarliestDate =
              m
                .DateField()
                .ValueAsDate
                .Value
                .Date >= earliestDate;

            bool dateIsOnOrBeforeLatestDate =
              m
                .DateField()
                .ValueAsDate
                .Value
                .Date <= latestDate;

            return dateIsOnOrAfterEarliestDate && dateIsOnOrBeforeLatestDate;
          });
      }
      catch (MissingFieldException ex)
      {
        _logger.LogError(
          $"Exception while retrieving meets in date range \"{earliestDate:f}\" to \"{latestDate:f}\".",
          ClassName,
          ex);

        throw new MeetsServiceException(
          "Exception while retrieving meets in date range.",
          ex);
      }
    }

    private IMeetsGoogleSheet CreateMeetSheet()
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

      return _googleSheetFactory.CreateSheet(
        uri,
        _requestMaker,
        _logger);
    }

    private async Task RetrieveMeetSheet(IMeetsGoogleSheet sheet)
    {
      _logger.LogDebug("Retrieving all meets...", ClassName);

      try
      {
        bool result = await sheet.Retrieve();

        if (!result)
        {
          throw new MeetsServiceException("Failed to retrieve all meets.");
        }
      }
      catch (MeetsGoogleSheetFormatException ex)
      {
        _logger.LogError(
          "Exception while retrieving all meets.",
          ClassName,
          ex);

        throw new MeetsServiceException("Exception while retrieving all meets.", ex);
      }
    }
  }
}

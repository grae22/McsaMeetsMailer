using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

namespace McsaMeetsMailer.Services
{
  public class MeetsService : IMeetsService
  {
    private static readonly string LoggingClassName = $"[{typeof(MeetsService).Name}]";

    private const string SettingName_MeetsGoogleSheetId = "MCSA-KZN_Meets_MeetsGoogleSheetId";
    private const string SettingName_GoogleAppKey = "MCSA-KZN_Meets_GoogleAppKey";
    private const string GoogleSheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

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

    public async Task<IEnumerable<MeetDetailsModel>> RetrieveMeets()
    {
      var url = $"{GoogleSheetsBaseUrl}{_meetsGoogleSheetId}/values/Sheet1!A1:Z500?key={_googleAppKey}";

      Uri uri;

      try
      {
        uri = new Uri(url);
      }
      catch (UriFormatException ex)
      {
        _logger.LogError($"{LoggingClassName} Failed to build URI from URL \"{url}\".", ex);
        return null;
      }

      _logger.LogDebug($"{LoggingClassName} Retrieving meets from \"{url}\"...");

      IMeetsGoogleSheet sheet = _googleSheetFactory.CreateSheet(
        uri,
        _requestMaker,
        _logger);

      bool result = await sheet.Retrieve();

      if (result == false)
      {
        _logger.LogError($"{LoggingClassName} Failed to retrieve meets.");
        return null;
      }

      _logger.LogDebug($"{LoggingClassName} Retrieved meets, transforming into models...");

      GoogleSheetToMeetDetailsTransformer.Process(
        sheet,
        out IEnumerable<MeetDetailsModel> meetDetailsModels);

      return meetDetailsModels;
    }
  }
}

using System;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

namespace McsaMeetsMailer.Services
{
  public class EmailAddressService : IEmailAddressService
  {
    private static readonly string ClassName = typeof(EmailAddressService).Name;

    private const string SettingName_EmailAddressGoogleSheetId = "MCSA-KZN_Meets_EmailAddressGoogleSheetId";
    private const string SettingName_GoogleAppKey = "MCSA-KZN_Meets_GoogleAppKey";
    private const string GoogleSheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
    private const string SheetRange = "A1:A1000";

    private readonly string _meetsGoogleSheetId;
    private readonly string _googleAppKey;
    private readonly IRestRequestMaker _requestMaker;
    private readonly IEmailAddressGoogleSheetFactory _googleSheetFactory;
    private readonly ILogger _logger;

    public EmailAddressService(
      in ISettings settings,
      in IRestRequestMaker requestMaker,
      in IEmailAddressGoogleSheetFactory googleSheetFactory,
      in ILogger logger)
    {
      if (settings == null)
      {
        throw new ArgumentNullException(nameof(settings));
      }

      _requestMaker = requestMaker ?? throw new ArgumentNullException(nameof(requestMaker));
      _googleSheetFactory = googleSheetFactory ?? throw new ArgumentNullException(nameof(googleSheetFactory));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _meetsGoogleSheetId = settings.GetValidValue(SettingName_EmailAddressGoogleSheetId);
      _googleAppKey = settings.GetValidValue(SettingName_GoogleAppKey);
    }

    public async Task<IEmailAddresses> RetrieveEmailAddresses()
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
        return null;
      }

      _logger.LogDebug($"Retrieving email addresses from \"{url}\"...", ClassName);

      IEmailAddressGoogleSheet sheet = _googleSheetFactory.CreateSheet(
        uri,
        _requestMaker,
        _logger);

      bool result = await sheet.Retrieve();

      if (result == false)
      {
        _logger.LogError("Failed to retrieve email addresses.", ClassName);
        return null;
      }

      return sheet.EmailAddresses;
    }
  }
}

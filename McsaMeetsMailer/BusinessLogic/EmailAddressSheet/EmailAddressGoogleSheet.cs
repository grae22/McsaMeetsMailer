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
    public const string ColumnHeader_FullSchedule = "Full Schedule";

    public IEnumerable<IEnumerable<string>> DataByRow => _dataByRow;

    private static readonly string ClassName = typeof(EmailAddressGoogleSheet).Name;

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
  }
}

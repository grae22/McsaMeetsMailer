using System;
using System.Collections.Generic;
using System.Linq;

using McsaMeetsMailer.Models;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Extensions;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McsaMeetsMailer.Pages
{
  public class MeetSheetLeaderPreview : PageModel
  {
    public IEnumerable<string> LeaderNames => _leaderNames;

    private readonly IMeetsService _meetsService;
    private readonly List<string> _leaderNames = new List<string>();

    public MeetSheetLeaderPreview(IMeetsService meetsService)
    {
      _meetsService = meetsService ?? throw new ArgumentNullException(nameof(meetsService));
    }

    public void OnGet()
    {
      IEnumerable<MeetDetailsModel> meets = _meetsService.RetrieveAllMeets().Result;

      PopulateLeaderNames(meets);
    }

    private void PopulateLeaderNames(in IEnumerable<MeetDetailsModel> meets)
    {
      _leaderNames.Clear();

      if (meets == null)
      {
        return;
      }

      meets
        .Select(l => l.LeaderField().Value)
        .ToList()
        .ForEach(l =>
        {
          if (!_leaderNames.Contains(l, StringComparer.OrdinalIgnoreCase))
          {
            _leaderNames.Add(l);
          }
        });
    }
  }
}
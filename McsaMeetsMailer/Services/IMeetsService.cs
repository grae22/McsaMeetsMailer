using System.Collections.Generic;
using System.Threading.Tasks;

using McsaMeetsMailer.Models;

namespace McsaMeetsMailer.Services
{
  public interface IMeetsService
  {
    Task<IEnumerable<MeetDetailsModel>> RetrieveMeets();
    Task<IEnumerable<MeetDetailsModel>> RetrieveMeets(string leaderName);
  }
}

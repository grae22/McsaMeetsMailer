using System.Collections.Generic;

namespace McsaMeetsMailer.Models
{
  public class MeetDetailsModel
  {
    public string Leader { get; set; }
    public string LeaderEmail { get; set; }
    public Dictionary<string, string> AdditionalFields { get; set; }
  }
}

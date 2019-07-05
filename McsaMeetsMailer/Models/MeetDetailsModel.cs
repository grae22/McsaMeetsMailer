using System.Collections.Generic;

namespace McsaMeetsMailer.Models
{
  public class MeetDetailsModel
  {
    public IEnumerable<MeetField> AllFields { get; set; }
    public IEnumerable<MeetFieldValue> FieldValues { get; set; }
  }
}

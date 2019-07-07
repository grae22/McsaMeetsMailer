using System.Collections.Generic;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;

namespace McsaMeetsMailer.Models
{
  public class MeetDetailsModel
  {
    public IEnumerable<MeetField> AllFields { get; set; }
    public IEnumerable<MeetFieldValue> FieldValues { get; set; }
  }
}

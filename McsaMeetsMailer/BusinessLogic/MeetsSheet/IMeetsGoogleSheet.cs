using System.Collections.Generic;
using System.Threading.Tasks;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public interface IMeetsGoogleSheet
  {
    IEnumerable<MeetField> Fields { get; }
    IEnumerable<IEnumerable<MeetFieldValue>> ValuesByRow { get; }

    Task<bool> Retrieve();

    int FindHeaderIndex(
      in string rawHeaderText,
      in bool raiseExceptionIfNotFound);
  }
}

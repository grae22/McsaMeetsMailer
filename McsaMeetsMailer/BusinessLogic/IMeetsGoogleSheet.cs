using System.Collections.Generic;
using System.Threading.Tasks;

namespace McsaMeetsMailer.BusinessLogic
{
  public interface IMeetsGoogleSheet
  {
    IEnumerable<string> Headers { get; }
    IEnumerable<IEnumerable<string>> DataByRow { get; }

    Task<bool> Retrieve();

    int FindHeaderIndex(
      in string headerText,
      in bool raiseExceptionIfNotFound);
  }
}

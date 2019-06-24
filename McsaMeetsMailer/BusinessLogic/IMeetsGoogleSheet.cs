using System.Collections.Generic;

namespace McsaMeetsMailer.BusinessLogic
{
  public interface IMeetsGoogleSheet
  {
    IEnumerable<string> Headers { get; }
    IEnumerable<IEnumerable<string>> DataByRow { get; }

    int FindHeaderIndex(
      in string headerText,
      in bool raiseExceptionIfNotFound);
  }
}

using System.Collections.Generic;

namespace McsaMeetsMailer.Utils.Html
{
  public interface IHtmlBuilder
  {
    void StartTable();

    void EndTable();

    void AddHeadingRow(IEnumerable<string> headings);

    void AddRow(IEnumerable<string> values);

    string GetHtml();
  }
}

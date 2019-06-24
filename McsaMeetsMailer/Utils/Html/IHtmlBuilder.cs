using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace McsaMeetsMailer.Utils.Html
{
  public interface IHtmlBuilder
  {
    void StartTable();

    void EndTable();

    void AddHeadingRow( IEnumerable<string> headings );

    void AddRow( IEnumerable<string> values );

    string GetHtml();
  }
}

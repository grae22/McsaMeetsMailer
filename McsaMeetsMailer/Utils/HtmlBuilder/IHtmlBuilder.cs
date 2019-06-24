using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace McsaMeetsMailer.Utils.HtmlBuilder
{
  public interface IHtmlBuilder
  {
    void StartTable();

    void EndTable();

    void AddHeadingRow( List<string> headings );

    void AddRow( List<string> values );

    string GetHtml();
  }
}

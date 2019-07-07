using System;
using System.Collections.Generic;

namespace McsaMeetsMailer.Utils.Html
{
  public interface IHtmlBuilder
  {
    void StartTable();

    void EndTable();

    void AddHeadingRow(IEnumerable<string> headings);

    void AddRow(IEnumerable<string> values);

    void AddParagraph(string text);

    void AddLineBreak();

    void AddStyleSheet(string path);

    string GetHtml();
  }
}

using System.Collections.Generic;
using System.Text;

namespace McsaMeetsMailer.Utils.Html
{
  public class HtmlBuilder : IHtmlBuilder
  {
    StringBuilder _htmlBuilder;

    public HtmlBuilder()
    {
      _htmlBuilder = new StringBuilder();
    }

    public void StartTable()
    {
      _htmlBuilder.Append("<table>");
    }

    public void EndTable()
    {
      _htmlBuilder.Append("</table>");
    }

    public void AddHeadingRow(IEnumerable<string> headings)
    {
      _htmlBuilder.Append("<tr>");

      foreach (var heading in headings)
      {
        _htmlBuilder.Append($"<th>{heading}</th>");
      }

      _htmlBuilder.Append("</tr>");
    }

    public void AddRow(IEnumerable<string> values)
    {
      _htmlBuilder.Append("<tr>");

      foreach (var value in values)
      {
        _htmlBuilder.Append($"<td>{value}</td>");
      }

      _htmlBuilder.Append("</tr>");
    }

    public void AddParagraph(string text)
    {
      _htmlBuilder.Append($"<p>{text}</p>");
    }

    public void AddLineBreak()
    {
      _htmlBuilder.Append("<br>");
    }

    public void AddStyleSheet(string path)
    {
      _htmlBuilder.Append($"<link href=\"@Url.Content(\"{path}\" )\" rel=\"stylesheet\" type=\"text/css\" />");}

    public string GetHtml()
    {
      return _htmlBuilder.ToString();
    }
  }
}

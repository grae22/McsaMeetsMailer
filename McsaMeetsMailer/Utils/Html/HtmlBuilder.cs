using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public void AddHeadingRow( IEnumerable<string> headings)
    {
      _htmlBuilder.Append("<tr>");

      foreach( var heading in headings )
      {
        _htmlBuilder.Append($"<th>{heading}</th>");
      }

      _htmlBuilder.Append("</tr>");
    }

    public void AddRow( IEnumerable<string> values)
    {
      _htmlBuilder.Append( "<tr>" );

      foreach (var value in values)
      {
        _htmlBuilder.Append( $"<td>{value}</td>" );
      }

      _htmlBuilder.Append( "</tr>");
    }

    public string GetHtml()
    {
      return _htmlBuilder.ToString();
    }
  }
}

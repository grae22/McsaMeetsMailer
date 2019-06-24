using System.Collections.Generic;
using System.Text;
using McsaMeetsMailer.Utils.Html;
using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Html
{
  [TestFixture]
  public class HtmlBuilderTests
  {
    [Test]
    public void Test_StartTable()
    {
      // Arrange
      var htmlBuilder = new HtmlBuilder();

      // Act
      htmlBuilder.StartTable();
      var html = htmlBuilder.GetHtml();

      // Assert
      Assert.AreEqual( "<table>", html );
    }

    [Test]
    public void Test_EndTable()
    {
      // Arrange
      var htmlBuilder = new HtmlBuilder();

      // Act
      htmlBuilder.EndTable();
      var html = htmlBuilder.GetHtml();

      // Assert
      Assert.AreEqual( "</table>", html );
    }

    [Test]
    public void Test_AddHeadingRow()
    {
      // Arrange
      var htmlBuilder = new HtmlBuilder();
      var testHeadings = new List<string>();
      var numberOfItems = 4;

      for( int i = 1; i < numberOfItems; ++i )
      {
        testHeadings.Add( $"heading{i}" );
      }

      // Act
      htmlBuilder.AddHeadingRow( testHeadings );
      var html = htmlBuilder.GetHtml();

      // Assert
      var expectedHtml = new StringBuilder( "<tr>" );
      for( int i = 1; i < numberOfItems; ++i )
      {
        expectedHtml.Append( $"<th>heading{i}</th>" );
      }
      expectedHtml.Append( "</tr>" );

      Assert.AreEqual( expectedHtml.ToString(), html );
    }

    [Test]
    public void Test_AddRow()
    {
      // Arrange
      var htmlBuilder = new HtmlBuilder();
      var testHeadings = new List<string>();
      var numberOfItems = 4;

      for( int i = 1; i < numberOfItems; ++i )
      {
        testHeadings.Add( $"value{i}" );
      }

      // Act
      htmlBuilder.AddRow( testHeadings );
      var html = htmlBuilder.GetHtml();

      // Assert
      var expectedHtml = new StringBuilder( "<tr>" );
      for( int i = 1; i < numberOfItems; ++i )
      {
        expectedHtml.Append( $"<td>value{i}</td>" );
      }
      expectedHtml.Append( "</tr>" );

      Assert.AreEqual( expectedHtml.ToString(), html );
    }
  }
}
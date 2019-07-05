using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Html;
using NUnit.Framework;
using System.Collections.Generic;

namespace McsaMeetsMailerTests.BusinessLogic
{
  public class FullScheduleEmailBuilderTests
  {/*
    [Test]
    public void Test_Build()
    {
      // Arrange
      var meetDetails = new List<MeetDetailsModel>
      {
        new MeetDetailsModel
        {
          Leader = "test1",
          LeaderEmail = "test1@gmail.com",
          AdditionalFields = new Dictionary<string, string>
          {
            {"heading1", "value1" },
            {"heading2", "value2" }
          }
        },
        new MeetDetailsModel
        {
          Leader = "testxyz",
          LeaderEmail = "testxyz@gmail.com",
          AdditionalFields = new Dictionary<string, string>
          {
            {"heading1", "valuexyz" },
            {"heading2", "value2xyz" }
          }
        }
      };

      var htmlBuilder = new HtmlBuilder();
      htmlBuilder.StartTable();
      htmlBuilder.AddHeadingRow(new List<string> { "Leader", "Leader Email", "heading1", "heading2" });
      htmlBuilder.AddRow(new List<string> { "test1", "test1@gmail.com", "value1", "value2" });
      htmlBuilder.AddRow(new List<string> { "testxyz", "testxyz@gmail.com", "valuexyz", "value2xyz" });

      // Act
      var actual = FullScheduleEmailBuilder.Build(meetDetails, htmlBuilder);

      // Assert
      var expected = htmlBuilder.GetHtml();
      Assert.AreEqual(expected, actual);
    }*/
  }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic
{
  [TestFixture]
  public class MeetsGoogleSheetTests
  {
    private const string DateColumnText = "# Date";

    [Test]
    public async Task Retrieve_GivenSuccessfulRetrieval_ShouldReturnNotNull()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new []
          {
            new [] { "", "", "" },
            new [] { "", DateColumnText, "" }
          }
        });

      // Act.
      MeetsGoogleSheet result = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Assert.
      Assert.IsNotNull(result);
    }

    [Test]
    public async Task Retrieve_GivenSheetWithoutDateColumn_ShouldRaiseException()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet());

      // Act & Assert.
      try
      {
        await MeetsGoogleSheet.Retrieve(
          url,
          requestMaker,
          logger);
      }
      catch (MeetsGoogleSheetFormatException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }

    [Test]
    public async Task Retrieve_GivenSheetWithDateColumn_ShouldNotRaiseException()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new []
          {
            new [] { "", "", "" },
            new [] { "", DateColumnText, "" }
          }
        });

      // Act.
      await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);
      
      // Assert.
      Assert.Pass();
    }

    [Test]
    public async Task Retrieve_GivenSheetWithData_ShouldReturnObjectWithData()
    {
      // Arrange.
      string[] columnNames =
      {
        DateColumnText,
        "Title",
        "Leader Name"
      };

      string[] rowValues1 =
      {
        "2019-01-01",
        "Title 1",
        "Leader 1"
      };

      string[] rowValues2 =
      {
        "2019-01-02",
        "Title 2",
        "Leader 2"
      };

      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new []
          {
            columnNames,
            rowValues1,
            rowValues2
          }
        });

      // Act.
      MeetsGoogleSheet testObject = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);
      
      // Assert.
      Assert.AreEqual(columnNames[0], testObject.Headers.ElementAt(0));
      Assert.AreEqual(columnNames[1], testObject.Headers.ElementAt(1));
      Assert.AreEqual(columnNames[2], testObject.Headers.ElementAt(2));
      
      Assert.AreEqual(rowValues1[0], testObject.DataByRow.ElementAt(0).ElementAt(0));
      Assert.AreEqual(rowValues1[1], testObject.DataByRow.ElementAt(0).ElementAt(1));
      Assert.AreEqual(rowValues1[2], testObject.DataByRow.ElementAt(0).ElementAt(2));
      
      Assert.AreEqual(rowValues2[0], testObject.DataByRow.ElementAt(1).ElementAt(0));
      Assert.AreEqual(rowValues2[1], testObject.DataByRow.ElementAt(1).ElementAt(1));
      Assert.AreEqual(rowValues2[2], testObject.DataByRow.ElementAt(1).ElementAt(2));
    }

    [Test]
    public async Task Retrieve_GivenSheetWithDataAndEmptyRows_ShouldReturnObjectWithData()
    {
      // Arrange.
      string[] columnNames =
      {
        DateColumnText,
        "Title",
        "Leader Name"
      };

      string[] rowValues1 =
      {
        "2019-01-01",
        "Title 1",
        "Leader 1"
      };

      string[] rowValues2 =
      {
        "",
        "",
        ""
      };

      string[] rowValues3 =
      {
        "",
        "",
        ""
      };

      string[] rowValues4 =
      {
        "2019-01-02",
        "Title 2",
        "Leader 2"
      };

      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new []
          {
            columnNames,
            rowValues1,
            rowValues2,
            rowValues3,
            rowValues4
          }
        });

      // Act.
      MeetsGoogleSheet testObject = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);
      
      // Assert.
      Assert.AreEqual(columnNames[0], testObject.Headers.ElementAt(0));
      Assert.AreEqual(columnNames[1], testObject.Headers.ElementAt(1));
      Assert.AreEqual(columnNames[2], testObject.Headers.ElementAt(2));
      
      Assert.AreEqual(rowValues1[0], testObject.DataByRow.ElementAt(0).ElementAt(0));
      Assert.AreEqual(rowValues1[1], testObject.DataByRow.ElementAt(0).ElementAt(1));
      Assert.AreEqual(rowValues1[2], testObject.DataByRow.ElementAt(0).ElementAt(2));
      
      Assert.AreEqual(rowValues4[0], testObject.DataByRow.ElementAt(1).ElementAt(0));
      Assert.AreEqual(rowValues4[1], testObject.DataByRow.ElementAt(1).ElementAt(1));
      Assert.AreEqual(rowValues4[2], testObject.DataByRow.ElementAt(1).ElementAt(2));
    }

    [Test]
    public async Task Retrieve_GivenUnsuccessfulRetrieval_ShouldReturnNull()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Throws(new RestRequestException(string.Empty, null));

      // Act.
      MeetsGoogleSheet result = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Assert.
      Assert.IsNull(result);
    }
  }
}

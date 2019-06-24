using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic
{
  [TestFixture]
  public class MeetsGoogleSheetTests
  {
    private const string DateColumnText = MeetsGoogleSheet.HeaderText_Date;

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
    public async Task Retrieve_GivenSheetWithDataAndEmptyColumnsAndRows_ShouldReturnObjectWithData()
    {
      // Arrange.
      string[] columnNames =
      {
        "",
        DateColumnText,
        "Title",
        "Leader Name"
      };

      string[] rowValues1 =
      {
        "",
        "2019-01-01",
        "Title 1",
        "Leader 1"
      };

      string[] rowValues2 =
      {
        "",
        "",
        "",
        ""
      };

      string[] rowValues3 =
      {
        "",
        "",
        "",
        ""
      };

      string[] rowValues4 =
      {
        "",
        "2019-01-02",
        "",
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
      Assert.AreEqual(columnNames[1], testObject.Headers.ElementAt(0));
      Assert.AreEqual(columnNames[2], testObject.Headers.ElementAt(1));
      Assert.AreEqual(columnNames[3], testObject.Headers.ElementAt(2));
      
      Assert.AreEqual(rowValues1[1], testObject.DataByRow.ElementAt(0).ElementAt(0));
      Assert.AreEqual(rowValues1[2], testObject.DataByRow.ElementAt(0).ElementAt(1));
      Assert.AreEqual(rowValues1[3], testObject.DataByRow.ElementAt(0).ElementAt(2));
      
      Assert.AreEqual(rowValues4[1], testObject.DataByRow.ElementAt(1).ElementAt(0));
      Assert.AreEqual(rowValues4[2], testObject.DataByRow.ElementAt(1).ElementAt(1));
      Assert.AreEqual(rowValues4[3], testObject.DataByRow.ElementAt(1).ElementAt(2));
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

    [Test]
    public async Task Retrieve_GivenTestData_ShouldReturnObjectWithCorrectData()
    {
      // Arrange.
      string testDataText = File.ReadAllText("./TestData/GoogleSheetTestData.json");

      var testData = JsonConvert.DeserializeObject<GoogleSheet>(testDataText);

      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(testData);

      // Act.
      MeetsGoogleSheet result = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(DateColumnText, result.Headers.ElementAt(0));
      Assert.AreEqual("Notes", result.Headers.ElementAt(12));
      Assert.AreEqual("2019-7-1", result.DataByRow.ElementAt(0).ElementAt(0));
      Assert.AreEqual("Meet 1", result.DataByRow.ElementAt(0).ElementAt(4));
      Assert.AreEqual("2019-7-10", result.DataByRow.ElementAt(1).ElementAt(0));
      Assert.AreEqual("Meet 2", result.DataByRow.ElementAt(1).ElementAt(4));
    }

    [TestCase(DateColumnText, 0)]
    [TestCase("Title", 1)]
    [TestCase("Leader Name", 2)]
    public async Task FindHeaderIndex_GivenHeaderText_ShouldReturnCorrectIndex(string headerText, int expectedIndex)
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
          values = new[]
          {
            columnNames,
            rowValues1,
            rowValues2
          }
        });

      MeetsGoogleSheet testObject = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Act.
      int result = testObject.FindHeaderIndex(headerText);

      // Assert.
      Assert.AreEqual(expectedIndex, result);
    }

    [Test]
    public async Task FindHeaderIndex_GivenMissingValueAndRequiredToRaiseException_ShouldRaiseException()
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
          values = new[]
          {
            columnNames,
            rowValues1,
            rowValues2
          }
        });

      MeetsGoogleSheet testObject = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Act & Assert.
      Assert.Throws<MeetsGoogleSheetFormatException>(() =>
        testObject.FindHeaderIndex("missing header", true));
    }

    [Test]
    public async Task FindHeaderIndex_GivenMissingValueAndNotRequiredToRaiseException_ShouldReturnNegativeOne()
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
          values = new[]
          {
            columnNames,
            rowValues1,
            rowValues2
          }
        });

      MeetsGoogleSheet testObject = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);
      
      // Act.
      int result = testObject.FindHeaderIndex("missing header");

      // Assert.
      Assert.AreEqual(-1, result);
    }
  }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic.MeetsSheet
{
  [TestFixture]
  public class MeetsGoogleSheetTests
  {
    private const string DateColumnText = "# Date*";

    [Test]
    public async Task Retrieve_GivenSuccessfulRetrieval_ShouldReturnTrue()
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      bool result = await testObject.Retrieve();

      // Assert.
      Assert.IsTrue(result);
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act & Assert.
      try
      {
        await testObject.Retrieve();
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.AreEqual(columnNames[0], testObject.Fields.ElementAt(0).RawText);
      Assert.AreEqual(columnNames[1], testObject.Fields.ElementAt(1).RawText);
      Assert.AreEqual(columnNames[2], testObject.Fields.ElementAt(2).RawText);

      Assert.AreEqual(rowValues1[0], testObject.ValuesByRow.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual(rowValues1[1], testObject.ValuesByRow.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual(rowValues1[2], testObject.ValuesByRow.ElementAt(0).ElementAt(2).Value);

      Assert.AreEqual(rowValues2[0], testObject.ValuesByRow.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual(rowValues2[1], testObject.ValuesByRow.ElementAt(1).ElementAt(1).Value);
      Assert.AreEqual(rowValues2[2], testObject.ValuesByRow.ElementAt(1).ElementAt(2).Value);
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.AreEqual(rowValues1[1], testObject.ValuesByRow.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual(rowValues1[2], testObject.ValuesByRow.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual(rowValues1[3], testObject.ValuesByRow.ElementAt(0).ElementAt(2).Value);

      Assert.AreEqual(rowValues4[1], testObject.ValuesByRow.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual(rowValues4[2], testObject.ValuesByRow.ElementAt(1).ElementAt(1).Value);
      Assert.AreEqual(rowValues4[3], testObject.ValuesByRow.ElementAt(1).ElementAt(2).Value);
    }

    [Test]
    public async Task Retrieve_GivenSheetSpecialFields_ShouldReturnObjectsWithPropertiesCorrectlySet()
    {
      // Arrange.
      string[] columnNames =
      {
        DateColumnText,
        "# Title",
        "Leader Name*"
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
            columnNames
          }
        });

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.IsTrue(testObject.Fields.ElementAt(0).DisplayInHeader);
      Assert.IsTrue(testObject.Fields.ElementAt(0).IsRequired);
      Assert.AreEqual(0, testObject.Fields.ElementAt(0).SortOrder);

      Assert.IsTrue(testObject.Fields.ElementAt(1).DisplayInHeader);
      Assert.IsFalse(testObject.Fields.ElementAt(1).IsRequired);
      Assert.AreEqual(1, testObject.Fields.ElementAt(1).SortOrder);

      Assert.IsFalse(testObject.Fields.ElementAt(2).DisplayInHeader);
      Assert.IsTrue(testObject.Fields.ElementAt(2).IsRequired);
      Assert.AreEqual(2, testObject.Fields.ElementAt(2).SortOrder);
    }

    [Test]
    public async Task Retrieve_GivenUnsuccessfulRetrieval_ShouldReturnFalse()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Throws(new RestRequestException(string.Empty, null));

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      bool result = await testObject.Retrieve();

      // Assert.
      Assert.IsFalse(result);
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.AreEqual(DateColumnText, testObject.Fields.ElementAt(0).RawText);
      Assert.AreEqual("Notes", testObject.Fields.ElementAt(12).RawText);
      Assert.AreEqual("2019-7-1", testObject.ValuesByRow.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual("Meet 1", testObject.ValuesByRow.ElementAt(0).ElementAt(4).Value);
      Assert.AreEqual("2019-7-10", testObject.ValuesByRow.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual("Meet 2", testObject.ValuesByRow.ElementAt(1).ElementAt(4).Value);
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      await testObject.Retrieve();

      // Act.
      int result = testObject.FindHeaderIndex(headerText);

      // Assert.
      Assert.AreEqual(expectedIndex, result);
    }

    [Test]
    public void FindHeaderIndex_GivenMissingValueAndRequiredToRaiseException_ShouldRaiseException()
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

      var testObject = new MeetsGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act & Assert.
      Assert.Throws<MeetsGoogleSheetFormatException>(() =>
        testObject.FindHeaderIndex("missing header", true));
    }

    [Test]
    public void FindHeaderIndex_GivenMissingValueAndNotRequiredToRaiseException_ShouldReturnNegativeOne()
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

      var testObject = new MeetsGoogleSheet(
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

using System;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic.EmailAddressSheet
{
  [TestFixture]
  public class EmailAddressGoogleSheetTests
  {
    private const string ColumnHeader_FullSchedule = "Full Schedule";

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
            new [] { $"{ColumnHeader_FullSchedule}", "", "" }
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      bool result = await testObject.Retrieve();

      // Assert.
      Assert.IsTrue(result);
    }

    [Test]
    public async Task Retrieve_GivenEmptySheet_ShouldRaiseException()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new[]
          {
            new string[] { }
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act & Assert.
      try
      {
        await testObject.Retrieve();
      }
      catch (EmailAddressGoogleSheetFormatException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }
    
    [Test]
    public async Task Retrieve_GivenTooFewColumns_ShouldRaiseException()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new[]
          {
            new[] { "" }
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act & Assert.
      try
      {
        await testObject.Retrieve();
      }
      catch (EmailAddressGoogleSheetFormatException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }

    [Test]
    public async Task Retrieve_GivenSheetCorrectColumnHeaders_ShouldNotRaiseException()
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
            new [] { ColumnHeader_FullSchedule },
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.Pass();
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

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      bool result = await testObject.Retrieve();

      // Assert.
      Assert.IsFalse(result);
    }

    [Test]
    public async Task Retrieve_GivenFullScheduleEmailAddresses_ShouldReturnAddresses()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      const string address1 = "abc";
      const string address2 = "def";

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new[]
          {
            new [] { $"{ColumnHeader_FullSchedule}" },
            new [] { $"{address1}" },
            new [] { $"{address2}" }
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.AreEqual(2, testObject.FullScheduleEmailAddresses.Count());
      Assert.AreEqual(address1, testObject.FullScheduleEmailAddresses.ElementAt(0));
      Assert.AreEqual(address2, testObject.FullScheduleEmailAddresses.ElementAt(1));
    }

    [Test]
    public async Task Retrieve_GivenFullScheduleEmailAddressesWithEmptyCells_ShouldExcludeEmptyValues()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      const string address1 = "abc";
      const string address2 = "def";

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet
        {
          values = new[]
          {
            new [] { $"{ColumnHeader_FullSchedule}" },
            new [] { $"{address1}" },
            new [] { string.Empty },
            new [] { $"{address2}" }
          }
        });

      var testObject = new EmailAddressGoogleSheet(
        url,
        requestMaker,
        logger);

      // Act.
      await testObject.Retrieve();

      // Assert.
      Assert.AreEqual(2, testObject.FullScheduleEmailAddresses.Count());
      Assert.AreEqual(address1, testObject.FullScheduleEmailAddresses.ElementAt(0));
      Assert.AreEqual(address2, testObject.FullScheduleEmailAddresses.ElementAt(1));
    }
  }
}

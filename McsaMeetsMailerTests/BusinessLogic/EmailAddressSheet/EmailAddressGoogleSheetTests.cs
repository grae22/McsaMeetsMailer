using System;
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
    private const string ColumnHeader_FullSchedule = EmailAddressGoogleSheet.ColumnHeader_FullSchedule;

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
    [Ignore("WIP")]
    public async Task Retrieve_GivenSheetWithoutFullScheduleColumn_ShouldRaiseException()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet());

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
            new [] { "", ColumnHeader_FullSchedule, "" }
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
  }
}

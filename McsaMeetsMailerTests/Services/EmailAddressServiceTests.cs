using System;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Services
{
  [TestFixture]
  public class EmailAddressServiceTests
  {
    [Test]
    public async Task RetrieveMeets_GivenHappyPath_ShouldReturnNotNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IEmailAddressGoogleSheetFactory>();
      var googleSheet = Substitute.For<IEmailAddressGoogleSheet>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      googleSheet
        .Retrieve()
        .Returns(true);

      var testObject = new EmailAddressService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveEmailAddresses();

      // Assert.
      Assert.IsNotNull(result);
    }

    [Test]
    public async Task RetrieveMeets_GivenRetrievalFail_ShouldReturnNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IEmailAddressGoogleSheetFactory>();
      var googleSheet = Substitute.For<IEmailAddressGoogleSheet>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      googleSheet
        .Retrieve()
        .Returns(false);

      var testObject = new EmailAddressService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveEmailAddresses();

      // Assert.
      Assert.IsNull(result);
    }

    [Test]
    public async Task RetrieveMeets_GivenFormatException_ShouldReturnNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IEmailAddressGoogleSheetFactory>();
      var googleSheet = Substitute.For<IEmailAddressGoogleSheet>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidValue(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      googleSheet
        .Retrieve()
        .Throws(new EmailAddressGoogleSheetFormatException(string.Empty));

      var testObject = new EmailAddressService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveEmailAddresses();

      // Assert.
      Assert.IsNull(result);
    }
  }
}

using System;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Services.Exceptions;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;
using McsaMeetsMailer.Utils.Validation.Validators;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Services
{
  [TestFixture]
  public class MeetsServiceTests
  {
    [Test]
    public async Task RetrieveAllMeets_GivenHappyPath_ShouldReturnNotNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
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

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveAllMeets();

      // Assert.
      Assert.IsNotNull(result);
    }

    [Test]
    public async Task RetrieveAllMeets_GivenRetrievalFail_ShouldRaiseMeetsServiceException()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
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

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act & Assert.
      try
      {
        await testObject.RetrieveAllMeets();
      }
      catch (MeetsServiceException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }

    [Test]
    public async Task RetrieveAllMeets_GivenMeetsSheetFormatException_ShouldReturnMeetsServiceException()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
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
        .Throws(new MeetsGoogleSheetFormatException(string.Empty));

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act & Assert.
      try
      {
        await testObject.RetrieveAllMeets();
      }
      catch (MeetsServiceException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }

    [Test]
    public async Task RetrieveMeets_GivenLeaderName_ShouldOnlyReturnMeetsForSpecifiedLeader()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
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

      var leaderField = new MeetField(
        false,
        false,
        "Leader",
        "Leader",
        0,
        false);

      googleSheet
        .Retrieve()
        .Returns(true);

      googleSheet
        .Fields
        .Returns(new[]
        {
          leaderField
        });

      googleSheet
        .ValuesByRow
        .Returns(
          new[]
          {
            new[] { new MeetFieldValue(leaderField, "Leader A", new ValidatorChain()) },
            new[] { new MeetFieldValue(leaderField, "Leader B", new ValidatorChain()) },
            new[] { new MeetFieldValue(leaderField, "Leader A", new ValidatorChain()) }
          });

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets("Leader A");

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task RetrieveMeets_GivenDate_ShouldOnlyReturnMeetsAfterSpecifiedDate()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
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

      var dateField = new MeetField(
        false,
        false,
        "Date",
        "Date",
        0,
        false);

      googleSheet
        .Retrieve()
        .Returns(true);

      googleSheet
        .Fields
        .Returns(new[]
        {
          dateField
        });

      googleSheet
        .ValuesByRow
        .Returns(
          new[]
          {
            new[] { new MeetFieldValue(dateField, "2019-7-15", new ValidatorChain()) },
            new[] { new MeetFieldValue(dateField, "2019-7-31", new ValidatorChain()) },
            new[] { new MeetFieldValue(dateField, "2019-7-1", new ValidatorChain()) }
          });

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets(new DateTime(2019, 7, 15));

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }
  }
}

﻿using System;
using System.Linq;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Services.Exceptions;
using McsaMeetsMailer.Utils.Formatting;
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
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
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
        dateTimeService,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets();

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
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
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
        dateTimeService,
        logger);

      // Act & Assert.
      try
      {
        await testObject.RetrieveMeets();
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
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
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
        dateTimeService,
        logger);

      // Act & Assert.
      try
      {
        await testObject.RetrieveMeets();
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
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();
      var formatter = NullFormatter.Instance();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      var leaderField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Leader",
        "Leader",
        0,
        false,
        formatter);

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
        dateTimeService,
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
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();
      var formatter = NullFormatter.Instance();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

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
        dateTimeService,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets(new DateTime(2019, 7, 15));

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task RetrieveMeets_GivenMeetWithInvalidDate_ShouldIncludeMeetWithInvalidDate()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();
      var formatter = NullFormatter.Instance();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

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
            new[] { new MeetFieldValue(dateField, "2019-7-x", new ValidatorChain()) },
            new[] { new MeetFieldValue(dateField, "2019-7-1", new ValidatorChain()) }
          });

      var testObject = new MeetsService(
        settings,
        requestMaker,
        googleSheetFactory,
        dateTimeService,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets(new DateTime(2019, 7, 15));

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task RetrieveMeets_GivenDateRange_ShouldOnlyReturnMeetsInRange()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
      var dateTimeService = Substitute.For<IDateTimeService>();
      var logger = Substitute.For<ILogger>();
      var formatter = NullFormatter.Instance();

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeSheetId");

      settings
        .GetValidString(Arg.Any<string>())
        .Returns("SomeAppKey");

      googleSheetFactory
        .CreateSheet(
          Arg.Any<Uri>(),
          Arg.Any<IRestRequestMaker>(),
          Arg.Any<ILogger>())
        .Returns(googleSheet);

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

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
        dateTimeService,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets(
        new DateTime(2019, 7, 15),
        new DateTime(2019, 7, 31));

      // Assert.
      Assert.IsNotNull(result);
      Assert.AreEqual(2, result.Count());
    }
  }
}

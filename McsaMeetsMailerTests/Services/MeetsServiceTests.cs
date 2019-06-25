using System;
using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic;
using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Logging;
using McsaMeetsMailer.Utils.RestRequest;
using McsaMeetsMailer.Utils.Settings;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Services
{
  [TestFixture]
  public class MeetsServiceTests
  {
    private const string SettingName_MeetsGoogleSheetId = "MCSA-KZN_Meets_MeetsGoogleSheetId";
    private const string SettingName_GoogleAppKey = "MCSA-KZN_Meets_GoogleAppKey";

    [Test]
    public async Task RetrieveMeets_GivenHappyPath_ShouldReturnNotNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(SettingName_MeetsGoogleSheetId)
        .Returns("SomeSheetId");

      settings
        .GetValidValue(SettingName_GoogleAppKey)
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
      var result = await testObject.RetrieveMeets();

      // Assert.
      Assert.IsNotNull(result);
    }

    [Test]
    public async Task RetrieveMeets_GivenRetrievalFail_ShouldReturnNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var googleSheetFactory = Substitute.For<IMeetsGoogleSheetFactory>();
      var googleSheet = Substitute.For<IMeetsGoogleSheet>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(SettingName_MeetsGoogleSheetId)
        .Returns("SomeSheetId");

      settings
        .GetValidValue(SettingName_GoogleAppKey)
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

      // Act.
      var result = await testObject.RetrieveMeets();

      // Assert.
      Assert.IsNull(result);
    }
  }
}

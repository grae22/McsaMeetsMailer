using System.Threading.Tasks;
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
    [Ignore("WIP")]
    public async Task RetrieveMeets_GivenHappyPath_ShouldReturnNotNullMeetsCollection()
    {
      // Arrange.
      var settings = Substitute.For<ISettings>();
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      settings
        .GetValidValue(SettingName_MeetsGoogleSheetId)
        .Returns("SomeSheetId");

      settings
        .GetValidValue(SettingName_GoogleAppKey)
        .Returns("SomeAppKey");



      var testObject = new MeetsService(
        settings,
        requestMaker,
        logger);

      // Act.
      var result = await testObject.RetrieveMeets();

      // Assert.
      Assert.IsNotNull(result);
    }
  }
}

using System;
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
    [Test]
    public async Task Retrieve_GivenSuccessfulRetrieval_ShouldReturnNotNull()
    {
      // Arrange.
      var url = new Uri("https://somegooglesheet");
      var requestMaker = Substitute.For<IRestRequestMaker>();
      var logger = Substitute.For<ILogger>();

      requestMaker
        .Get<GoogleSheet>(url)
        .Returns(new GoogleSheet());

      // Act.
      MeetsGoogleSheet result = await MeetsGoogleSheet.Retrieve(
        url,
        requestMaker,
        logger);

      // Assert.
      Assert.IsNotNull(result);
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

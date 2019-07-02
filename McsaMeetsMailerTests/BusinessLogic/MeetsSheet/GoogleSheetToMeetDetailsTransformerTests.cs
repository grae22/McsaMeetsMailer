using System.Collections.Generic;
using System.Linq;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic.MeetsSheet
{
  [TestFixture]
  public class GoogleSheetToMeetDetailsTransformerTests
  {
    [Test]
    public void Process_GivenGoogleSheet_ShouldOutputMeetDetails()
    {
      // Arrange.
      var sheet = Substitute.For<IMeetsGoogleSheet>();
      
      sheet
        .Headers
        .Returns(new[]
        {
          "Column 1",
          "Column 2",
          MeetsGoogleSheet.HeaderText_LeaderName,
          "Column 3",
          MeetsGoogleSheet.HeaderText_LeaderEmail
        });

      sheet
        .DataByRow
        .Returns(new[]
        {
          new [] { "R1C1", "R1C2", "R1C3", "R1C4", "R1C5" },
          new [] { "R2C1", "R2C2", "R2C3", "R2C4", "R2C5" }
        });

      sheet
        .FindHeaderIndex(MeetsGoogleSheet.HeaderText_LeaderName, Arg.Any<bool>())
        .Returns(2);

      sheet
        .FindHeaderIndex(MeetsGoogleSheet.HeaderText_LeaderEmail, Arg.Any<bool>())
        .Returns(4);

      // Act.
      GoogleSheetToMeetDetailsTransformer.Process(
        sheet,
        out IEnumerable<MeetDetailsModel> models);

      // Assert.
      Assert.IsNotNull(models);
      Assert.AreEqual(2, models.Count());
      Assert.AreEqual("R1C3", models.ElementAt(0).Leader);
      Assert.AreEqual("R2C3", models.ElementAt(1).Leader);
      Assert.AreEqual("R1C5", models.ElementAt(0).LeaderEmail);
      Assert.AreEqual("R2C5", models.ElementAt(1).LeaderEmail);
      Assert.AreEqual("R1C1", models.ElementAt(0).AdditionalFields["Column 1"]);
      Assert.AreEqual("R2C1", models.ElementAt(1).AdditionalFields["Column 1"]);
      Assert.AreEqual("R1C4", models.ElementAt(0).AdditionalFields["Column 3"]);
      Assert.AreEqual("R2C4", models.ElementAt(1).AdditionalFields["Column 3"]);
    }
  }
}

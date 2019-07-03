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
        .Fields
        .Returns(new[]
        {
          new MeetField(false, false, string.Empty, "Column 1", 0),
          new MeetField(false, false, string.Empty, "Column 2", 0),
          new MeetField(false, false, string.Empty, MeetsGoogleSheet.HeaderText_LeaderName, 0),
          new MeetField(false, false, string.Empty, "Column 3", 0),
          new MeetField(false, false, string.Empty, MeetsGoogleSheet.HeaderText_LeaderEmail, 0),
        });

      sheet
        .ValuesByRow
        .Returns(new[]
        {
          new[]
          {
            new MeetFieldValue(null, "R1C1"),
            new MeetFieldValue(null, "R1C2"),
            new MeetFieldValue(null, "R1C3"),
            new MeetFieldValue(null, "R1C4"),
            new MeetFieldValue(null, "R1C5")
          },
          new[]
          {
            new MeetFieldValue(null, "R2C1"),
            new MeetFieldValue(null, "R2C2"),
            new MeetFieldValue(null, "R2C3"),
            new MeetFieldValue(null, "R2C4"),
            new MeetFieldValue(null, "R2C5")
          },
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

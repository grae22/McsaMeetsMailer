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
          new MeetField(false, false, string.Empty, "Column 1", 0, false),
          new MeetField(false, false, string.Empty, "Column 2", 0, false),
          new MeetField(false, false, string.Empty, "Column 3", 0, true),
          new MeetField(true, false, string.Empty, "Column 4", 0, false),
          new MeetField(false, true, string.Empty, "Column 5", 0, false),
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
        .FindHeaderIndex("Column 3", Arg.Any<bool>())
        .Returns(2);

      sheet
        .FindHeaderIndex("Column 5", Arg.Any<bool>())
        .Returns(4);

      // Act.
      GoogleSheetToMeetDetailsTransformer.Process(
        sheet,
        out IEnumerable<MeetDetailsModel> models);

      // Assert.
      Assert.IsNotNull(models);

      Assert.AreEqual(2, models.Count());

      Assert.AreEqual("R1C1", models.ElementAt(0).FieldValues.ElementAt(0).Value);
      Assert.AreEqual("R2C5", models.ElementAt(1).FieldValues.ElementAt(4).Value);

      Assert.AreEqual(5, models.ElementAt(0).AllFields.Count());

      Assert.AreEqual("Column 1", models.ElementAt(0).AllFields.ElementAt(0).FriendlyText);
      Assert.AreEqual("Column 5", models.ElementAt(0).AllFields.ElementAt(4).FriendlyText);

      Assert.IsTrue(models.ElementAt(0).AllFields.ElementAt(3).DisplayInHeader);
      Assert.IsTrue(models.ElementAt(0).AllFields.ElementAt(4).IsRequired);
      Assert.IsTrue(models.ElementAt(0).AllFields.ElementAt(2).IsMeetTitle);
    }
  }
}

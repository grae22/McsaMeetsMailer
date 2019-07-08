using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Validation.Validators;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Extensions
{
  [TestFixture]
  public class MeetDetailsModelExtensionsTests
  {
    [Test]
    public void LeaderName_GivenLeaderNameFieldFound_ShouldReturnLeaderName()
    {
      // Arrange.
      var dateField = new MeetField(
        false,
        false,
        "Date",
        "Date",
        0,
        false);

      var leaderField = new MeetField(
        false,
        false,
        "Leader",
        "Leader",
        0,
        false);

      var testObject = new MeetDetailsModel
      {
        FieldValues = new[]
        {
          new MeetFieldValue(dateField, string.Empty, new ValidatorChain()),
          new MeetFieldValue(leaderField, "Leader Name", new ValidatorChain())
        }
      };

      // Act.
      MeetFieldValue result = testObject.LeaderField();

      // Assert.
      Assert.NotNull(result);
      Assert.AreEqual("Leader Name", result.Value);
    }

    [Test]
    public void LeaderName_GivenLeaderNameFieldNotFound_ShouldRaiseException()
    {
      // Arrange.
      var dateField = new MeetField(
        false,
        false,
        "Date",
        "Date",
        0,
        false);

      var notesField = new MeetField(
        false,
        false,
        "Notes",
        "Notes",
        0,
        false);

      var testObject = new MeetDetailsModel
      {
        FieldValues = new[]
        {
          new MeetFieldValue(dateField, string.Empty, new ValidatorChain()),
          new MeetFieldValue(notesField, string.Empty, new ValidatorChain())
        }
      };

      // Act & Assert.
      try
      {
        testObject.LeaderField();
      }
      catch (MissingMeetFieldException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }
  }
}

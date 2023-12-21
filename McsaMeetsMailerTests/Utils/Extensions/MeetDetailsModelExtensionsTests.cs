using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.Extensions;
using McsaMeetsMailer.Utils.Formatting;
using McsaMeetsMailer.Utils.Validation.Validators;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Extensions
{
  [TestFixture]
  public class MeetDetailsModelExtensionsTests
  {
    [Test]
    public void LeaderField_GivenLeaderNameFieldFound_ShouldReturnLeaderName()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

      var leaderField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Leader",
        "Leader",
        0,
        false,
        formatter);

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
    public void LeaderField_GivenLeaderNameFieldNotFound_ShouldRaiseException()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

      var notesField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Notes",
        "Notes",
        0,
        false,
        formatter);

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

    [Test]
    public void DateField_GivenDateFieldFound_ShouldReturnDate()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Date",
        "Date",
        0,
        false,
        formatter);

      var leaderField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Leader",
        "Leader",
        0,
        false,
        formatter);

      var testObject = new MeetDetailsModel
      {
        FieldValues = new[]
        {
          new MeetFieldValue(dateField, "2019-7-1", new ValidatorChain()),
          new MeetFieldValue(leaderField, "Leader Name", new ValidatorChain())
        }
      };

      // Act.
      MeetFieldValue result = testObject.DateField();

      // Assert.
      Assert.NotNull(result);
      Assert.AreEqual("2019-7-1", result.Value);
    }

    [Test]
    public void DateField_GivenDateFieldNotFound_ShouldRaiseException()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var dateField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Leader",
        "Leader",
        0,
        false,
        formatter);

      var notesField = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        "Notes",
        "Notes",
        0,
        false,
        formatter);

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
        testObject.DateField();
      }
      catch (MissingMeetFieldException)
      {
        Assert.Pass();
      }

      Assert.Fail();
    }
  }
}

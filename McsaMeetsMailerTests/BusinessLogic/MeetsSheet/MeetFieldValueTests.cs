using System;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Utils.Validation.Validators;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.BusinessLogic.MeetsSheet
{
  [TestFixture]
  public class MeetFieldValueTests
  {
    [Test]
    public void ValueAsDate_GivenValueIsValidDate_ShouldReturnDate()
    {
      // Arrange.
      var field = new MeetField(
        false,
        false,
        string.Empty,
        string.Empty,
        0,
        false);

      var testObject = new MeetFieldValue(
        field,
        "2019-7-1",
        new ValidatorChain());

      // Act.
      DateTime? result = testObject.ValueAsDate;

      // Assert.
      Assert.IsNotNull(result);

      Assert.AreEqual(
        new DateTime(2019, 7, 1),
        result);
    }

    [Test]
    public void ValueAsDate_GivenValueIsInvalidDate_ShouldReturnNull()
    {
      // Arrange.
      var field = new MeetField(
        false,
        false,
        string.Empty,
        string.Empty,
        0,
        false);

      var testObject = new MeetFieldValue(
        field,
        "2019-7-x",
        new ValidatorChain());

      // Act.
      DateTime? result = testObject.ValueAsDate;

      // Assert.
      Assert.IsNull(result);
    }

    [Test]
    public void ValueAsDate_GivenValueIsEmpty_ShouldReturnNull()
    {
      // Arrange.
      var field = new MeetField(
        false,
        false,
        string.Empty,
        string.Empty,
        0,
        false);

      var testObject = new MeetFieldValue(
        field,
        string.Empty,
        new ValidatorChain());

      // Act.
      DateTime? result = testObject.ValueAsDate;

      // Assert.
      Assert.IsNull(result);
    }
  }
}

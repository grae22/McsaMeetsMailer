using System;

using McsaMeetsMailer.BusinessLogic.MeetsSheet;
using McsaMeetsMailer.Utils.Formatting;
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
      var formatter = Substitute.For<IFormatter>();

      var field = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        string.Empty,
        string.Empty,
        0,
        false,
        formatter);

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

    [Ignore("?")]
    [Test]
    public void ValueAsDate_GivenValueIsInvalidDate_ShouldReturnNull()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var field = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        string.Empty,
        string.Empty,
        0,
        false,
        formatter);

      var testObject = new MeetFieldValue(
        field,
        "2019-7-x",
        new ValidatorChain());

      // Act.
      DateTime? result = testObject.ValueAsDate;

      // Assert.
      Assert.IsNull(result);
    }

    [Ignore("?")]
    [Test]
    public void ValueAsDate_GivenValueIsEmpty_ShouldReturnNull()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var field = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        string.Empty,
        string.Empty,
        0,
        false,
        formatter);

      var testObject = new MeetFieldValue(
        field,
        string.Empty,
        new ValidatorChain());

      // Act.
      DateTime? result = testObject.ValueAsDate;

      // Assert.
      Assert.IsNull(result);
    }

    [Test]
    public void FormattedValue_GivenValue_ShouldReturnFormattedValue()
    {
      // Arrange.
      var formatter = Substitute.For<IFormatter>();

      var field = new MeetField(
        MeetField.HeaderStatusType.ExcludeFromHeader,
        false,
        string.Empty,
        string.Empty,
        0,
        false,
        formatter);

      formatter
        .Format(Arg.Any<string>())
        .Returns("123");

      // Act.
      var testObject = new MeetFieldValue(
        field,
        "321",
        new ValidatorChain());

      // Assert.
      Assert.AreEqual("123", testObject.FormattedValue);
    }
  }
}

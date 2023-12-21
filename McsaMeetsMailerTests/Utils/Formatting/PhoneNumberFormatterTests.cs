using McsaMeetsMailer.Utils.Formatting;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Formatting
{
  public class PhoneNumberFormatterTests
  {
    [TestCase("123456789")]
    [TestCase("0123456789")]
    [TestCase("012 345 6789")]
    [TestCase("012 3456789")]
    public void GivenTenDigitPhoneNumber_WhenFormatted_ThenOutputIsCorrect(
      in string number)
    {
      // Arrange.
      var testObject = new PhoneNumberFormatter();

      // Act.
      string output = testObject.Format(number);

      // Assert.
      Assert.That(output, Is.EqualTo("012 345 6789"));
    }

    [TestCase("27123456789")]
    [TestCase("+27123456789")]
    [TestCase("+27123456789")]
    [TestCase("+27 12 345 6789")]
    [TestCase("+27 123456789")]
    public void GivenTwelveDigitPhoneNumber_WhenFormatted_ThenOutputIsCorrect(
      in string number)
    {
      // Arrange.
      var testObject = new PhoneNumberFormatter();

      // Act.
      string output = testObject.Format(number);

      // Assert.
      Assert.That(output, Is.EqualTo("+27 12 345 6789"));
    }

    [TestCase("")]
    [TestCase("012345666")]
    [TestCase("0123456789666")]
    [TestCase("+27123456789666")]
    public void GivenUnknownFormatPhoneNumber_WhenFormatted_ThenOutputIsSameAsInput(
      in string number)
    {
      // Arrange.
      var testObject = new PhoneNumberFormatter();

      // Act.
      string output = testObject.Format(number);

      // Assert.
      Assert.That(output, Is.EqualTo(number));
    }
  }
}

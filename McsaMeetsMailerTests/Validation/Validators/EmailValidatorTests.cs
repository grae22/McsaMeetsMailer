using McsaMeetsMailer.Utils.Validation.Validators;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Validation.Validators
{
  [TestFixture]
  public class EmailValidatorTests
  {
    [TestCase("a@b.com")]
    [TestCase("a.b@b.co.za")]
    [TestCase("a_b@b.co.za")]
    public void Validate_GivenValidEmailAddress_ShouldReturnTrue(in string emailAddress)
    {
      // Arrange.
      var testObject = new EmailValidator();

      // Act.
      bool result = testObject.Validate(emailAddress);

      // Assert.
      Assert.IsTrue(result);
      Assert.IsTrue(testObject.IsValid);
    }

    [TestCase("ab.com")]
    [TestCase("ab@c com")]
    [TestCase("@c.com")]
    public void Validate_GivenInvalidEmailAddress_ShouldReturnFalse(in string emailAddress)
    {
      // Arrange.
      var testObject = new EmailValidator();

      // Act.
      bool result = testObject.Validate(emailAddress);

      // Assert.
      Assert.IsFalse(result);
      Assert.IsFalse(testObject.IsValid);
    }
  }
}

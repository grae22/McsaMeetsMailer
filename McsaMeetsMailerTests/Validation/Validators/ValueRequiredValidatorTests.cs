using McsaMeetsMailer.Utils.Validation.Validators;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Validation.Validators
{
  [TestFixture]
  public class ValueRequiredValidatorTests
  {
    [Test]
    public void Validate_GivenEmptyString_ShouldReturnFalse()
    {
      // Arrange.
      var testObject = new ValueRequiredValidator();

      // Act.
      bool result = testObject.Validate(string.Empty);

      // Assert.
      Assert.IsFalse(result);
      Assert.IsFalse(testObject.IsValid);
    }
  }
}

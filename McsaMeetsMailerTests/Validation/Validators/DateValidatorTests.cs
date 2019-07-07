using McsaMeetsMailer.Utils.Validation.Validators;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Validation.Validators
{
  [TestFixture]
  public class DateValidatorTests
  {
    [Test]
    public void Validate_GivenInvalidValue_ShouldReturnFalse()
    {
      // Arrange.
      var testObject = new DateValidator();
      var input = "2019-abcd";

      // Act.
      bool result = testObject.Validate(in input);

      // Assert.
      Assert.IsFalse(result);
      Assert.IsFalse(testObject.IsValid);
      Assert.AreEqual($"Date or time \"{input}\" is invalid.", testObject.ErrorMessage);
    }
    
    [Test]
    public void Validate_GivenValidValue_ShouldReturnTrue()
    {
      // Arrange.
      var testObject = new DateValidator();
      var input = "2019-07-01";

      // Act.
      bool result = testObject.Validate(in input);

      // Assert.
      Assert.IsTrue(result);
      Assert.IsTrue(testObject.IsValid);
      Assert.AreEqual(string.Empty, testObject.ErrorMessage);
    }
    
    [Test]
    public void Validate_GivenValidTimeValue_ShouldReturnTrue()
    {
      // Arrange.
      var testObject = new DateValidator();
      var input = "23:59:59";

      // Act.
      bool result = testObject.Validate(in input);

      // Assert.
      Assert.IsTrue(result);
      Assert.IsTrue(testObject.IsValid);
      Assert.AreEqual(string.Empty, testObject.ErrorMessage);
    }

    [Test]
    public void Validate_GivenInValidTimeValue_ShouldReturnFalse()
    {
      // Arrange.
      var testObject = new DateValidator();
      var input = "24:00";

      // Act.
      bool result = testObject.Validate(in input);

      // Assert.
      Assert.IsFalse(result);
      Assert.IsFalse(testObject.IsValid);
      Assert.AreEqual($"Date or time \"{input}\" is invalid.", testObject.ErrorMessage);
    }
  }
}

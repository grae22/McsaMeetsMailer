using McsaMeetsMailer.Utils.Validation.Validators;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Validation.Validators
{
  [TestFixture]
  public class GradeValidatorTests
  {
    [TestCase("1 & 2")]
    [TestCase("2")]
    [TestCase("3")]
    [TestCase("4")]
    [TestCase("5")]
    [TestCase("1 2")]
    [TestCase("1 & 2")]
    [TestCase("1,2")]
    [TestCase("1, 2 & 5")]
    public void IsValid_GivenValidInput_ShouldReturnTrue(in string input)
    {
      // Arrange.
      var testObject = new GradeValidator();

      // Act.
      bool result = testObject.Validate(input);

      // Assert.
      Assert.IsTrue(result);
      Assert.IsTrue(testObject.IsValid);
    }

    [TestCase("0")]
    [TestCase("a")]
    [TestCase("1 & a")]
    [TestCase("1, 2 & a")]
    public void IsValid_GivenInvalidInput_ShouldReturnFalse(in string input)
    {
      // Arrange.
      var testObject = new GradeValidator();

      // Act.
      bool result = testObject.Validate(input);

      // Assert.
      Assert.IsFalse(result);
      Assert.IsFalse(testObject.IsValid);
    }
  }
}

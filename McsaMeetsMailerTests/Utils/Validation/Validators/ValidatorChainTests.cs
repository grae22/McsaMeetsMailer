using McsaMeetsMailer.Utils.Validation.Validators;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Validation.Validators
{
  [TestFixture]
  public class ValidatorChainTests
  {
    [Test]
    public void Validate_GivenNoValidators_ShouldReturnTrue()
    {
      // Arrange.
      var testObject = new ValidatorChain();

      // Act.
      var input = string.Empty;

      testObject.Validate(input);

      // Assert.
      Assert.IsTrue(testObject.IsValid);
    }

    [Test]
    public void Validate_GivenOneValidatorWhichPasses_ShouldReturnTrue()
    {
      // Arrange.
      var testObject = new ValidatorChain();
      var validator = Substitute.For<IValidator>();

      validator
        .IsValid
        .Returns(true);

      testObject.AddValidator(validator);

      // Act.
      var input = string.Empty;

      testObject.Validate(input);

      // Assert.
      Assert.IsTrue(testObject.IsValid);
    }
    
    [Test]
    public void Validate_GivenSeveralValidators_ShouldErrorMessageForFirstFailed()
    {
      // Arrange.
      var testObject = new ValidatorChain();
      var validator1 = Substitute.For<IValidator>();
      var validator2 = Substitute.For<IValidator>();
      var validator3 = Substitute.For<IValidator>();

      validator1
        .IsValid
        .Returns(true);

      validator2
        .IsValid
        .Returns(false);

      validator3
        .IsValid
        .Returns(false);

      validator2
        .ErrorMessage
        .Returns("222");

      validator3
        .ErrorMessage
        .Returns("333");

      testObject.AddValidator(validator1);
      testObject.AddValidator(validator2);
      testObject.AddValidator(validator3);

      // Act.
      var input = string.Empty;

      testObject.Validate(input);

      // Assert.
      Assert.IsFalse(testObject.IsValid);
      Assert.AreEqual("222", testObject.ErrorMessage);
    }
  }
}

using System;
using McsaMeetsMailer.Utils.Validation;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Validation
{
  [TestFixture]
  public class CommonValidationTests
  {
    [Test]
    public void RaiseExceptionIfArgumentNull_GivenNullArgument_ShouldRaiseArgumentNullException()
    {
      // Arrange.
      // Act.
      // Assert.
      Assert.Throws<ArgumentNullException>(
        () => CommonValidation.RaiseExceptionIfArgumentNull(null, "SomeName"));
    }

    [Test]
    public void RaiseExceptionIfArgumentNull_GivenNotNullArgument_ShouldNotRaiseException()
    {
      // Arrange.
      // Act.
      CommonValidation.RaiseExceptionIfArgumentNull("SomeValue", "SomeName");

      // Assert.
      Assert.Pass();
    }
  }
}

using McsaMeetsMailer.Utils.Formatting;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Formatting
{
  [TestFixture]
  public class DateFormatterTests
  {
    [Test]
    public void Format_GivenDateString_ShouldReturnFormattedString()
    {
      // Arrange.
      var testObject = new DateFormatter("d MMM (ddd)");

      // Act.
      string result = testObject.Format("2019-7-1");
      
      // Assert.
      Assert.AreEqual("1 Jul (Mon)", result);
    }

    [Test]
    public void Format_GivenNonDateString_ShouldReturnStringUnchanged()
    {
      // Arrange.
      var testObject = new DateFormatter("d MMM (ddd)");

      const string input = "2019-7-x";

      // Act.
      string result = testObject.Format(input);
      
      // Assert.
      Assert.AreEqual(input, result);
    }
  }
}

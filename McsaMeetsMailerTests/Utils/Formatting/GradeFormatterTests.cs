using McsaMeetsMailer.Utils.Formatting;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Formatting
{
  [TestFixture]
  public class GradeFormatterTests
  {
    [TestCase("1", "Family Friendly")]
    [TestCase("2", "Easy Hike")]
    [TestCase("3", "Serious Hike")]
    [TestCase("4", "Very Serious Hike")]
    [TestCase("5", "Rock Climbing")]
    public void Format_GivenGrade_ShouldFormatWithCorrectFriendlyText(
      in string input,
      in string expectedFriendlyText)
    {
      // Arrange.
      var testObject = new GradeFormatter();
      
      // Act.
      string result = testObject.Format(input);

      // Assert.
      Assert.AreEqual($"{input} ({expectedFriendlyText})", result);
    }

    [Test]
    public void Format_GivenUnknownGrade_ShouldReturnUnchanged()
    {
      // Arrange.
      var testObject = new GradeFormatter();
      
      // Act.
      string result = testObject.Format("abc");

      // Assert.
      Assert.AreEqual("abc", result);
    }

    [TestCase("1 & 2", "1 (Family Friendly), 2 (Easy Hike)")]
    [TestCase("1 & 2", "1 (Family Friendly), 2 (Easy Hike)")]
    [TestCase("1 2", "1 (Family Friendly), 2 (Easy Hike)")]
    [TestCase("1,2", "1 (Family Friendly), 2 (Easy Hike)")]
    [TestCase("1, 2 & 999", "1 (Family Friendly), 2 (Easy Hike), 999")]
    public void Format_GivenMultipleGrades_ShouldFormatEachWithCorrectFriendlyText(
      in string input,
      in string expected)
    {
      // Arrange.
      var testObject = new GradeFormatter();

      // Act.
      string result = testObject.Format(input);

      // Assert.
      Assert.AreEqual(expected, result);
    }
  }
}

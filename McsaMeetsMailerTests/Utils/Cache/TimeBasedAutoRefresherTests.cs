using System;
using System.Threading.Tasks;

using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Cache;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Cache
{
  [TestFixture]
  public class TimeBasedAutoRefresherTests
  {
    [Test]
    public async Task Instance_GivenFirstCall_ShouldRefreshAndReturnValue()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;
      const string value = "abc";

      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var refreshable = new Refreshable();

      time
        .Now
        .Returns(timeAtInstantiation);

      var testObject = new TimeBasedAutoRefresher<Refreshable>(
        refreshable,
        time,
        cacheLifetimeInSeconds,
        () => refreshable.Refresh(value));

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds - 1));

      // Act.
      Refreshable result = await testObject.Instance();

      // Assert.
      Assert.AreEqual(value, result.Value);
    }
    
    [Test]
    public async Task Instance_GivenExpired_ShouldRefreshAndReturnNewValue()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;

      string[] values = { "abc", "def" };
      int valueIndex = 0;

      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var refreshable = new Refreshable();

      time
        .Now
        .Returns(timeAtInstantiation);

      var testObject = new TimeBasedAutoRefresher<Refreshable>(
        refreshable,
        time,
        cacheLifetimeInSeconds,
        () => refreshable.Refresh(values[valueIndex++]));

      await testObject.Instance();

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds));

      // Act.
      Refreshable result = await testObject.Instance();

      // Assert.
      Assert.AreEqual(values[1], result.Value);
    }
    
    [Test]
    public async Task Instance_GivenUnexpired_ShouldRefreshAndReturnOldValue()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;

      string[] values = { "abc", "def" };
      int valueIndex = 0;

      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var refreshable = new Refreshable();

      time
        .Now
        .Returns(timeAtInstantiation);

      var testObject = new TimeBasedAutoRefresher<Refreshable>(
        refreshable,
        time,
        cacheLifetimeInSeconds,
        () => refreshable.Refresh(values[valueIndex++]));

      await testObject.Instance();

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds - 1));

      // Act.
      Refreshable result = await testObject.Instance();

      // Assert.
      Assert.AreEqual(values[0], result.Value);
    }

    private class Refreshable
    {
      public string Value { get; private set; }

      public async Task Refresh(string value)
      {
        await Task.Delay(0);
        Value = value;
      }
    }
  }
}

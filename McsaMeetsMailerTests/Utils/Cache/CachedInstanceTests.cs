using System;

using McsaMeetsMailer.Services;
using McsaMeetsMailer.Utils.Cache;

using NSubstitute;

using NUnit.Framework;

namespace McsaMeetsMailerTests.Utils.Cache
{
  [TestFixture]
  public class CachedObjectTests
  {
    [Test]
    public void Instance_GivenInstance_ShouldReturnInstance()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;

      var instance = "abc";
      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var instanceSource = Substitute.For<Func<string>>();

      time
        .Now
        .Returns(timeAtInstantiation);

      instanceSource
        .Invoke()
        .Returns(
          instance,
          "def");

      var testObject = new CachedInstance<string>(
        time,
        cacheLifetimeInSeconds,
        instanceSource);

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds - 1));

      // Act.
      string result = testObject.Instance;

      // Assert.
      Assert.AreSame(instance, result);
    }

    [Test]
    public void Instance_GivenExpiredInstance_ShouldReturnNewInstance()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;

      var instance = "abc";
      var newInstance = "def";
      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var instanceSource = Substitute.For<Func<string>>();

      time
        .Now
        .Returns(timeAtInstantiation);

      instanceSource
        .Invoke()
        .Returns(
          instance,
          newInstance);

      var testObject = new CachedInstance<string>(
        time,
        cacheLifetimeInSeconds,
        () => newInstance);

      var unused = testObject.Instance;

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds));

      // Act.
      string result = testObject.Instance;

      // Assert.
      Assert.AreSame(newInstance, result);
    }
    
    [Test]
    public void Instance_GivenUnexpiredInstance_ShouldReturnOldInstance()
    {
      // Arrange.
      const int cacheLifetimeInSeconds = 5;

      var instance = "abc";
      var newInstance = "def";
      var time = Substitute.For<IDateTimeService>();
      var timeAtInstantiation = new DateTime();
      var instanceSource = Substitute.For<Func<string>>();

      time
        .Now
        .Returns(timeAtInstantiation);

      instanceSource
        .Invoke()
        .Returns(
          instance,
          newInstance);

      var testObject = new CachedInstance<string>(
        time,
        cacheLifetimeInSeconds,
        instanceSource);

      var unused = testObject.Instance;

      time
        .Now
        .Returns(timeAtInstantiation.AddSeconds(cacheLifetimeInSeconds - 1));

      // Act.
      string result = testObject.Instance;

      // Assert.
      Assert.AreSame(instance, result);
    }
  }
}

using System;

using McsaMeetsMailer.Services;

namespace McsaMeetsMailer.Utils.Cache
{
  public class CachedInstance<T> where T : class
  {
    public T Instance => GetValidInstance();

    private readonly IDateTimeService _dateTimeService;
    private readonly uint _cacheLifetimeInSeconds;
    private readonly Func<T> _newInstance;

    private T _instance;
    private DateTime _expiryTime;

    public CachedInstance(
      in IDateTimeService dateTimeService,
      in uint cacheLifetimeInSeconds,
      in Func<T> instanceSource)
    {
      _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
      _cacheLifetimeInSeconds = cacheLifetimeInSeconds;
      _newInstance = instanceSource ?? throw new ArgumentNullException(nameof(instanceSource));
    }

    private void SetInstance(in T instance)
    {
      _instance = instance ?? throw new ArgumentNullException(nameof(instance));
      _expiryTime = _dateTimeService.Now.AddSeconds(_cacheLifetimeInSeconds);
    }

    private void GetNewInstanceIfNoneExists()
    {
      if (_instance != null)
      {
        return;
      }

      UpdateTheInstance();
    }

    private void GetNewInstanceIfExpired()
    {
      if (_dateTimeService.Now < _expiryTime)
      {
        return;
      }

      UpdateTheInstance();
    }

    private T GetValidInstance()
    {
      GetNewInstanceIfNoneExists();
      GetNewInstanceIfExpired();

      return _instance;
    }

    private void UpdateTheInstance()
    {
      T newInstance = _newInstance.Invoke();

      SetInstance(newInstance);
    }
  }
}

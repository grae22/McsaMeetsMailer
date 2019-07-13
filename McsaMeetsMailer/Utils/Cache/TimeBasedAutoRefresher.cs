using System;
using System.Threading.Tasks;

using McsaMeetsMailer.Services;

namespace McsaMeetsMailer.Utils.Cache
{
  public class TimeBasedAutoRefresher<T> where T : class
  {
    private readonly IDateTimeService _dateTimeService;
    private readonly uint _cacheLifetimeInSeconds;
    private readonly Func<Task> _refresh;
    private readonly T _instance;

    private DateTime _expiryTime;

    public TimeBasedAutoRefresher(
      in T instance,
      in IDateTimeService dateTimeService,
      in uint cacheLifetimeInSeconds,
      in Func<Task> refresh)
    {
      _instance = instance ?? throw new ArgumentNullException(nameof(instance));
      _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
      _cacheLifetimeInSeconds = cacheLifetimeInSeconds;
      _refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
    }

    public async Task<T> Instance()
    {
      return await GetValidInstance();
    }

    private async Task<T> GetValidInstance()
    {
      await PerformRefreshIfRequired();

      return _instance;
    }

    private async Task PerformRefreshIfRequired()
    {
      if (_dateTimeService.Now < _expiryTime)
      {
        return;
      }

      await Refresh();
    }

    private async Task Refresh()
    {
      await _refresh.Invoke();

      UpdateExpiry();
    }

    private void UpdateExpiry()
    {
      _expiryTime = _dateTimeService.Now.AddSeconds(_cacheLifetimeInSeconds);
    }

  }
}

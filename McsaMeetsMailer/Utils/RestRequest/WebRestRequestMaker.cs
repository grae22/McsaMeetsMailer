using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace McsaMeetsMailer.Utils.RestRequest
{
  internal class WebRestService : IRestRequestMaker
  {
    public async Task<T> Get<T>(Uri address)
    {
      const int maxAttempts = 5;

      if (address == null)
      {
        throw new ArgumentNullException(nameof(address));
      }

      Exception lastException = null;

      for (int i = 0; i < maxAttempts; i++)
      {
        try
        {
          using (var client = new HttpClient())
          {
            using (var response = await client.GetAsync(address))
            {
              using (var content = response.Content)
              {
                string data = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(data);
              }
            }
          }
        }
        catch (Exception ex)
        {
          lastException = ex;

          await Task.Delay(1000);
        }
      }

      throw new RestRequestException("Data retrieval failed", lastException);
    }

    public async Task<bool> Put(Uri address, string content)
    {
      const int maxAttempts = 5;

      if (address == null)
      {
        throw new ArgumentNullException(nameof(address));
      }

      var httpContent = new StringContent(content);

      httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

      Exception lastException = null;

      for (int i = 0; i < maxAttempts; i++)
      {
        try
        {
          using (var client = new HttpClient())
          {
            using (var response = await client.PutAsync(address, httpContent))
            {
              return response.IsSuccessStatusCode;
            }
          }
        }
        catch (Exception ex)
        {
          lastException = ex;

          await Task.Delay(1000);
        }
      }

      throw new RestRequestException("Data update failed", lastException);
    }
  }
}
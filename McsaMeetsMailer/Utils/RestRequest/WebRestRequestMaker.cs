using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace McsaMeetsMailer.Utils.RestRequest
{
  internal class WebRestService : IRestRequestMaker
  {
    private static Uri _address;

    public WebRestService(in Uri address)
    {
      _address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public async Task<T> Get<T>()
    {
      const int maxAttempts = 5;

      for (int i = 0; i < maxAttempts; i++)
      {
        try
        {
          using (var client = new HttpClient())
          {
            using (var response = await client.GetAsync(_address))
            {
              using (var content = response.Content)
              {
                string data = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(data);
              }
            }
          }
        }
        catch (Exception)
        {
          if (i == maxAttempts - 1)
          {
            throw;
          }

          await Task.Delay(1000);
        }
      }

      throw new Exception("Data retrieval failed");
    }

    public async Task<bool> Put(string content)
    {
      var httpContent = new StringContent(content);

      httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

      const int maxAttempts = 5;

      for (int i = 0; i < maxAttempts; i++)
      {
        try
        {
          using (var client = new HttpClient())
          {
            using (var response = await client.PutAsync(_address, httpContent))
            {
              return response.IsSuccessStatusCode;
            }
          }
        }
        catch (Exception)
        {
          if (i == maxAttempts - 1)
          {
            throw;
          }

          await Task.Delay(1000);
        }
      }

      throw new Exception("Data retrieval failed");
    }
  }
}
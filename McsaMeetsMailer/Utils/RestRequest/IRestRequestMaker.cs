using System;
using System.Threading.Tasks;

namespace McsaMeetsMailer.Utils.RestRequest
{
  public interface IRestRequestMaker
  {
    Task<T> Get<T>(Uri address);
    Task<bool> Put(Uri address, string content);
  }
}
using System.Threading.Tasks;

namespace McsaMeetsMailer.Utils.RestRequest
{
  public interface IRestRequestMaker
  {
    Task<T> Get<T>();
    Task<bool> Put(string content);
  }
}
using System.Threading.Tasks;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public interface IEmailAddressGoogleSheet
  {
    Task<bool> Retrieve();
  }
}
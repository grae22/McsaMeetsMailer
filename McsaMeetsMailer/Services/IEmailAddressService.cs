using System.Threading.Tasks;

using McsaMeetsMailer.BusinessLogic.EmailAddressSheet;

namespace McsaMeetsMailer.Services
{
  public interface IEmailAddressService
  {
    Task<IEmailAddresses> RetrieveEmailAddresses();
  }
}

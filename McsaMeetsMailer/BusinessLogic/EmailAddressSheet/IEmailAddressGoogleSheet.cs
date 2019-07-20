using System.Threading.Tasks;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public interface IEmailAddressGoogleSheet
  {
    IEmailAddresses EmailAddresses { get; }

    Task<bool> Retrieve();
  }
}
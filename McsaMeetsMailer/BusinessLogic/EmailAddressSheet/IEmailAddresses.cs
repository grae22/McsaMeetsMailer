using System.Collections.Generic;

namespace McsaMeetsMailer.BusinessLogic.EmailAddressSheet
{
  public interface IEmailAddresses
  {
    IEnumerable<string> FullScheduleEmailAddresses { get; }
  }
}
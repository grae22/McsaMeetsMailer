using System;

namespace McsaMeetsMailer.Services
{
  public class DateTimeService : IDateTimeService
  {
    public DateTime Now => DateTime.Now;
  }
}

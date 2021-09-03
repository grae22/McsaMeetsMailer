namespace McsaMeetsMailer.Models
{
  public class EmailContent
  {
    public string Address { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string SendMeetsUpUntilThisDate { get; set; }
  }
}

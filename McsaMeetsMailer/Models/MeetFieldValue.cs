namespace McsaMeetsMailer.Models
{
  public class MeetFieldValue
  {
    public MeetField Field { get; }
    public string Value { get; }

    public MeetFieldValue(
      in MeetField field,
      in string value)
    {
      Field = field;
      Value = value;
    }
  }
}

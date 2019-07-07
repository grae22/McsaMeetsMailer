namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MeetField
  {
    public bool DisplayInHeader { get; }
    public bool IsRequired { get; }
    public string RawText { get; }
    public string FriendlyText { get; }
    public int SortOrder { get; }
    public bool IsMeetTitle { get; }

    public MeetField(
      in bool displayInHeader,
      in bool isRequired,
      in string rawText,
      in string friendlyText,
      in int sortOrder,
      in bool isMeetTitle)
    {
      DisplayInHeader = displayInHeader;
      IsRequired = isRequired;
      RawText = rawText;
      FriendlyText = friendlyText;
      SortOrder = sortOrder;
      IsMeetTitle = isMeetTitle;
    }
  }
}

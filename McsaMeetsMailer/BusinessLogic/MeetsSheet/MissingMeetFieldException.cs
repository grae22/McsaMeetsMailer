using System;

namespace McsaMeetsMailer.BusinessLogic.MeetsSheet
{
  public class MissingMeetFieldException : Exception
  {
    public MissingMeetFieldException(in string fieldName)
    :
      base($"Failed to find field with name \"{fieldName}\".")
    {
    }
  }
}

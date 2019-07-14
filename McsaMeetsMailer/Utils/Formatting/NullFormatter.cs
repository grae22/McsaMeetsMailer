namespace McsaMeetsMailer.Utils.Formatting
{
  public class NullFormatter : IFormatter
  {
    public static NullFormatter Instance()
    {
      if (_staticInstance == null)
      {
        _staticInstance = new NullFormatter();
      }

      return _staticInstance;
    }

    private static NullFormatter _staticInstance;

    // Use the static instance.
    private NullFormatter()
    {
    }

    public string Format(in string input)
    {
      return input;
    }
  }
}

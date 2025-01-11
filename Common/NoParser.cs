namespace Common;

public class NoParser : IParser<string[]>
{
  public string[] Parse(string[] input) => input;
}

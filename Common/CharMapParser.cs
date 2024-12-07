namespace Common;

public class CharMapParser : IParser<char[,]>
{
  public char[,] Parse(string[] input) => Parsing.ParseToCharMap(input);
}

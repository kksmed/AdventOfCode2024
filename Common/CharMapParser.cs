using System.Drawing;

namespace Common;

public class CharMapParser : IParser<char[,]>
{
  public char[,] Parse(string[] input) => Parsing.ParseToCharMap(input);
}

public static class CharMapExtensions
{
  public static bool InBounds(this char[,] map, Point p) =>
    p.X >= 0 && p.X < map.GetLength(0) && p.Y >= 0 && p.Y < map.GetLength(1);
}

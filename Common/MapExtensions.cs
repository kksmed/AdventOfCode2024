using System.Drawing;

namespace Common;

public static class MapExtensions
{
  public static bool InBounds<T>(this T[,] map, Point p) =>
    p.X >= 0 && p.X < map.GetLength(0) && p.Y >= 0 && p.Y < map.GetLength(1);
}

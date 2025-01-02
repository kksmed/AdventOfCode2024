using System.Drawing;

namespace Common;

public static class MapExtensions
{
  public static T Get<T>(this T[,] map, Point p) => map[p.X, p.Y];

  public static bool InBounds<T>(this T[,] map, Point p) =>
    p.X >= 0 && p.X < map.GetLength(0) && p.Y >= 0 && p.Y < map.GetLength(1);

  public static IEnumerable<Point> GetCheatNeighbours(this Point p, int maxDistance)
  {
    for (var yDiff = 1 - maxDistance; yDiff<maxDistance; yDiff++)
    for (var xDiff = 1 - maxDistance; xDiff < maxDistance; xDiff++)
    {
      if (Math.Abs(yDiff) + Math.Abs(xDiff) < maxDistance && !(yDiff == 0 && xDiff == 0))
        yield return new(p.X + xDiff, p.Y + yDiff);
    }
  }

  public static IEnumerable<Point> GetNeighbours(this Point p)
  {
    // Right
    yield return p with
    {
      X = p.X + 1
    };
    // Up
    yield return p with
    {
      Y = p.Y - 1
    };
    // Left
    yield return p with
    {
      X = p.X - 1
    };
    // Down
    yield return p with
    {
      Y = p.Y + 1
    };
  }

  public static T[,] Copy<T>(this T[,] map) where T : ICopyable<T>
  {
    var copy = new T[map.GetLength(0), map.GetLength(1)];
    for (var y = 0; y < map.GetLength(1); y++)
    for (var x = 0; x < map.GetLength(0); x++)
      copy[x, y] = map[x, y].Copy();

    return copy;
  }
}

using System.Drawing;
using Common;

namespace Day12;

class Solver2Alternative : ISolver<char[,], int>
{
  public int Solve(char[,] data) => GetFields(data).Sum(x => x.Count * CountEdges(x));

  static int CountEdges(HashSet<Point> area)
  {
    var minX = area.Min(p => p.X);
    var maxX = area.Max(p => p.X);
    var minY = area.Min(p => p.Y);
    var maxY = area.Max(p => p.Y);

    var edgeCount = 0;

    // Vertical edges
    for (var x = minX; x <= maxX; x++)
    {
      var ongoingEdgeLeft = false;
      var ongoingEdgeRight = false;
      for (var y = minY; y <= maxY; y++)
      {
        var p = new Point(x, y);
        if (!area.Contains(p))
        {
          ongoingEdgeLeft = false;
          ongoingEdgeRight = false;
          continue;
        }

        var edgeToTheLeft = !area.Contains(p with { X = x - 1 });
        if (!ongoingEdgeLeft && edgeToTheLeft)
        {
          edgeCount++;
        }

        ongoingEdgeLeft = edgeToTheLeft;

        var edgeToTheRight = !area.Contains(p with { X = x + 1 });
        if (!ongoingEdgeRight && edgeToTheRight)
        {
          edgeCount++;
        }

        ongoingEdgeRight = edgeToTheRight;
      }
    }

    // Horizontal edges
    for (var y = minY; y <= maxY; y++)
    {
      var ongoingEdgeUp = false;
      var ongoingEdgeDown = false;

      for (var x = minX; x <= maxX; x++)
      {
        var p = new Point(x, y);
        if (!area.Contains(p))
        {
          ongoingEdgeUp = false;
          ongoingEdgeDown = false;
          continue;
        }

        var edgeUp = !area.Contains(p with { Y = y - 1 });
        if (!ongoingEdgeUp && edgeUp)
        {
          edgeCount++;
        }

        ongoingEdgeUp = edgeUp;

        var edgeDown = !area.Contains(p with { Y = y + 1 });
        if (!ongoingEdgeDown && edgeDown)
        {
          edgeCount++;
        }

        ongoingEdgeDown = edgeDown;
      }
    }

    return edgeCount;
  }

  static IEnumerable<HashSet<Point>> GetFields(char[,] map)
  {
    var used = new bool[map.GetLength(0), map.GetLength(1)];
    for (var x = 0; x < map.GetLength(0); x++)
    for (var y = 0; y < map.GetLength(1); y++)
    {
      if (used[x, y])
        continue;

      Point startPoint = new(x, y);
      var plant = map[startPoint.X, startPoint.Y];
      yield return [.. GetConnectedArea(startPoint, plant, Direction.Right)];
    }
    yield break;

    List<Point> GetConnectedArea(Point startPoint, char plant, Direction direction)
    {
      used[startPoint.X, startPoint.Y] = true;
      List<Point> area = [startPoint];

      foreach (
        var directionToGo in new[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down }.Where(d =>
          d != Opposite(direction)
        )
      )
      {
        var next = Go(startPoint, directionToGo);
        if (!map.InBounds(next) || map.Get(next) != plant || used[next.X, next.Y])
          continue;
        var subArea = GetConnectedArea(next, plant, directionToGo);
        area.AddRange(subArea);
      }

      return area;
    }
  }

  enum Direction
  {
    Up,
    Down,
    Left,
    Right,
  }

  static Point Go(Point p, Direction direction) =>
    direction switch
    {
      Direction.Up => p with { Y = p.Y - 1 },
      Direction.Down => p with { Y = p.Y + 1 },
      Direction.Left => p with { X = p.X - 1 },
      Direction.Right => p with { X = p.X + 1 },
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };

  static Direction Opposite(Direction direction) =>
    direction switch
    {
      Direction.Up => Direction.Down,
      Direction.Down => Direction.Up,
      Direction.Left => Direction.Right,
      Direction.Right => Direction.Left,
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };
}

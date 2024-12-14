using System.Drawing;

using Common;

var example =
  """
  RRRRIICCFF
  RRRRIICCCF
  VVRRRCCFFF
  VVRCCCJFFF
  VVVVCJJCFE
  VVIVCCJJEE
  VVIIICJJEE
  MIIIIIJJEE
  MIIISIJEEE
  MMMISSJEEE
  """;

Solving.Go(example, new CharMapParser(), new Solver());

class Solver : ISolver<char[,], int>
{

  public int Solve(char[,] data)
  {
    var costs = 0;
    var used = new bool[data.GetLength(0), data.GetLength(1)];
    foreach (var startPoint in GetUnused(used))
    {
      var plant = data[startPoint.X, startPoint.Y];
      var results = Search(data, startPoint, plant, used, Direction.Right);

      costs += (1 + results.Perimeter) * results.Area;
    }

    return costs;
  }

  static IEnumerable<Point> GetUnused(bool[,] data)
  {
    for (var x = 0; x < data.GetLength(0); x++)
    for (var y = 0; y < data.GetLength(1); y++)
    {
      if (data[x, y] == false)
        yield return new(x, y);
    }
  }

  static (int Perimeter, int Area) Search(char[,] map, Point p, char plant, bool[,] used, Direction direction)
  {
    used[p.X, p.Y] = true;
    var perimeter = 0;
    var area = 1;

    foreach (var directionToGo in new[]
    {
      Direction.Left, Direction.Up, Direction.Right, Direction.Down
    }.Where(d => d != Oppersite(direction)))
    {
      var next = Go(p, directionToGo);
      if (map.InBounds(next) && map[next.X, next.Y] == plant)
      {
        if (used[next.X, next.Y])
          continue;
        var subResults = Search(map, next, plant, used, directionToGo);
        perimeter += subResults.Perimeter;
        area += subResults.Area;
      }
      else
      {
        perimeter++;
      }
    }

    return (perimeter, area);
  }

  static Point Go(Point p, Direction direction) => direction switch
  {
    Direction.Up => p with
    {
      Y = p.Y - 1
    },
    Direction.Down => p with
    {
      Y = p.Y + 1
    },
    Direction.Left => p with
    {
      X = p.X - 1
    },
    Direction.Right => p with
    {
      X = p.X + 1
    },
    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
  };

  static Direction Oppersite(Direction direction) => direction switch
  {
    Direction.Up => Direction.Down,
    Direction.Down => Direction.Up,
    Direction.Left => Direction.Right,
    Direction.Right => Direction.Left,
    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
  };

  enum Direction { Up, Down, Left, Right }
}
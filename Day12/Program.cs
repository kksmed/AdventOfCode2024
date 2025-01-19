using System.Diagnostics;
using System.Drawing;
using Common;
using Day12;

var parser = new CharMapParser();
var s2 = new Solver2Alternative();

var example = """
  AAAA
  BBCD
  BBCC
  EEEC
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 80);

example = """
  OOOOO
  OXOXO
  OOOOO
  OXOXO
  OOOOO
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 436);

example = """
  EEEEE
  EXXXX
  EEEEE
  EXXXX
  EEEEE
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 236);

example = """
  AAAAAA
  AAABBA
  AAABBA
  ABBAAA
  ABBAAA
  AAAAAA
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 368);

example = """
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
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 1206);

example = """
  EEXXX
  XEEXX
  EEXXX
  XEXXX
  EEXXX
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 264);

example = """
  XXXXXXXXXXXXXXXXXXXXXXXXXXX
  XXXXXXXXXXXXXGGXXXXXXXGGGXX
  XXXXXXXXXXXXXXGXXXXXXXGGXXX
  XXXXXXXXXXXXGGGXXXXGGGGGXXX
  XXXXXXXXXXXGGGGGGXGGGGGGXXX
  XXXXXXXXXXXGGGGGGXXGGGGGGXX
  XXXXXXXXGGGGGGGGGGGGGGGGGGX
  XXXXXXXXGGGGGGGGGGGGGGGGGGX
  XXGGXXGGGGGGGGGGGGGGGGGGGXX
  XXGGXXGGGGGGGGGGGGGGGGGGXXX
  XXXGGGXGGGGGGGGGGGGGGXXGXXX
  XXXGGGGGGGGGGGGGGGGGGGGGXXX
  XXXGGGGGGGGGGGGGGGGGGGXGXXX
  XGGGGGGGGGGGGGXGGGGGGXXXXXX
  XXGGGGGGGGGGGGGGGGGGGXXXXXX
  XGGGGGGGGGGGGGXXXXXGXXXXXXX
  XXXGGGGGGGGGGXXXXXXXXXXXXXX
  XXXGGGGGGGGGGXXXXXXXXXXXXXX
  XXGGGGGGGGGGGGXXXXXXXXXXXXX
  XXGGGGGGGGXGXXXXXXXXXXXXXXX
  XXXXGGGGGXXGXXXXXXXXXXXXXXX
  XXXXGGGGGXXXXXXXXXXXXXXXXXX
  XXXXXGGGGXXXXXXXXXXXXXXXXXX
  XXXXXXXXXXXXXXXXXXXXXXXXXXX
  """;
s2.Solve(parser.Parse(example.Split(Environment.NewLine)));

example = """
  XXXXX
  XXEEX
  XXXEX
  XEEEX
  XXXXX
  """;
Debug.Assert(s2.Solve(parser.Parse(example.Split(Environment.NewLine))) == 276);

Console.WriteLine("All tests passed");

Solving.Go(example, parser, new Solver(), new Solver2Alternative());

class Solver : ISolver<char[,], int>
{
  public virtual int Solve(char[,] data)
  {
    var costs = 0;
    var used = new bool[data.GetLength(0), data.GetLength(1)];
    foreach (var startPoint in GetUnused(used))
    {
      var plant = data[startPoint.X, startPoint.Y];
      var results = GetPerimeterAndArea(data, startPoint, plant, used, Direction.Right);

      costs += (1 + results.Perimeter) * results.Area;
    }

    return costs;
  }

  protected static IEnumerable<Point> GetUnused(bool[,] data)
  {
    for (var x = 0; x < data.GetLength(0); x++)
    for (var y = 0; y < data.GetLength(1); y++)
    {
      if (data[x, y] == false)
        yield return new(x, y);
    }
  }

  static (int Perimeter, int Area) GetPerimeterAndArea(
    char[,] map,
    Point point,
    char plant,
    bool[,] used,
    Direction direction
  )
  {
    used[point.X, point.Y] = true;
    var perimeter = 0;
    var area = 1;

    foreach (
      var directionToGo in new[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down }.Where(d =>
        d != Opposite(direction)
      )
    )
    {
      var next = Go(point, directionToGo);
      if (map.InBounds(next) && map[next.X, next.Y] == plant)
      {
        if (used[next.X, next.Y])
          continue;
        var subResults = GetPerimeterAndArea(map, next, plant, used, directionToGo);
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

  protected static Point Go(Point p, Direction direction) =>
    direction switch
    {
      Direction.Up => p with { Y = p.Y - 1 },
      Direction.Down => p with { Y = p.Y + 1 },
      Direction.Left => p with { X = p.X - 1 },
      Direction.Right => p with { X = p.X + 1 },
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };

  protected static Direction Opposite(Direction direction) =>
    direction switch
    {
      Direction.Up => Direction.Down,
      Direction.Down => Direction.Up,
      Direction.Left => Direction.Right,
      Direction.Right => Direction.Left,
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };

  protected enum Direction
  {
    Up,
    Down,
    Left,
    Right,
  }
}

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

Solving.Go(example, new CharMapParser(), new Solver(), new Solver2());

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

  static (int Perimeter, int Area) GetPerimeterAndArea(char[,] map, Point point, char plant, bool[,] used, Direction direction)
  {
    used[point.X, point.Y] = true;
    var perimeter = 0;
    var area = 1;

    foreach (var directionToGo in new[]
    {
      Direction.Left, Direction.Up, Direction.Right, Direction.Down
    }.Where(d => d != Opposite(direction)))
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

  protected static Point Go(Point p, Direction direction) => direction switch
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

  protected static Direction Opposite(Direction direction) => direction switch
  {
    Direction.Up => Direction.Down,
    Direction.Down => Direction.Up,
    Direction.Left => Direction.Right,
    Direction.Right => Direction.Left,
    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
  };

  protected enum Direction { Up, Down, Left, Right }
}

class Solver2 : Solver
{
  public override int Solve(char[,] data)
  {
    var costs = 0;
    var used = new bool[data.GetLength(0), data.GetLength(1)];
    foreach (var startPoint in GetUnused(used))
    {
      var plant = data[startPoint.X, startPoint.Y];
      var results = GetSidesAndArea(data, startPoint, plant, used, default, Direction.Right);
      var down = Go(startPoint, Direction.Down);
      var extraSides = !data.InBounds(down) || data[down.X, down.Y] != plant? 1 : 0;

      costs += results.Area * (extraSides + results.Sides);
      Console.WriteLine($"{plant}: {results.Area} x {extraSides + results.Sides}");
    }

    return costs;
  }

  static (int Area, int Sides) GetSidesAndArea(char[,] map, Point point, char plant, bool[,] used, Edge edge, Direction direction)
  {
    used[point.X, point.Y] = true;
    var area = 1;
    var sides = 0;

    var goLeft = NextStep(point, TurnDirection(direction, Turn.Left));
    var goStraight = NextStep(point, direction);
    var goRight = NextStep(point, TurnDirection(direction, Turn.Right));

    if (goLeft.CanGo)
    {
      Edge newEdge = default;
      if (edge.HasFlag(Edge.LeftHandSide))
      {
        sides++;
        newEdge = Edge.LeftHandSide;
        Console.WriteLine($"At: {point} - {direction} - new side because turning left to {goLeft.Point}");
      }

      if (!used[goLeft.Point.X, goLeft.Point.Y])
      {
        if (!goStraight.CanGo)
          newEdge |= Edge.RightHandSide;
        var subResults = GetSidesAndArea(map, goLeft.Point, plant, used, newEdge, goLeft.Direction);
        sides += subResults.Sides;
        area += subResults.Area;
      }
    }
    else if (!edge.HasFlag(Edge.LeftHandSide))
    {
      sides++;
      Console.WriteLine($"At: {point} - {direction} - new side to the left ({goLeft.Point})");
    }

    if (goStraight.CanGo)
    {
      if (!used[goStraight.Point.X, goStraight.Point.Y])
      {
        var newEdge = (!goLeft.CanGo ? Edge.LeftHandSide : 0) | (!goRight.CanGo ? Edge.RightHandSide : default);
        var subResults = GetSidesAndArea(map, goStraight.Point, plant, used, newEdge, goStraight.Direction);
        sides += subResults.Sides;
        area += subResults.Area;
      }
    }
    else
    {
      sides++;
      Console.WriteLine($"At: {point} - {direction} - new side straight ({goStraight.Point})");
    }

    if (goRight.CanGo)
    {
      Edge newEdge = default;
      if (edge.HasFlag(Edge.RightHandSide))
      {
        sides++;
        newEdge = Edge.RightHandSide;
        Console.WriteLine($"At: {point} - {direction} - new side because turning right to {goRight.Point}");
      }

      if (!used[goRight.Point.X, goRight.Point.Y])
      {
        if (!goStraight.CanGo)
          newEdge |= Edge.LeftHandSide;

        var subResults = GetSidesAndArea(map, goRight.Point, plant, used, newEdge, goRight.Direction);
        sides += subResults.Sides;
        area += subResults.Area;
      }
    }
    else if (!edge.HasFlag(Edge.RightHandSide))
    {
      sides++;
      Console.WriteLine($"At: {point} - {direction} - new side to the right ({goRight.Point})");
    }

    return (Area: area, Sides: sides);

    (Point Point, Direction Direction, bool CanGo) NextStep(Point p, Direction d)
    {
      var nextPoint = Go(p, d);
      return (Point: nextPoint, Direction: d, CanGo: map.InBounds(nextPoint) && map[nextPoint.X, nextPoint.Y] == plant);
    }
  }

  static Direction TurnDirection(Direction direction, Turn turn)
  {
    return (turn, direction) switch
    {
      (Turn.Left, Direction.Up) => Direction.Left,
      (Turn.Left, Direction.Down) => Direction.Right,
      (Turn.Left, Direction.Right) => Direction.Up,
      (Turn.Left, Direction.Left) => Direction.Down,
      (Turn.Right, Direction.Up) => Direction.Right,
      (Turn.Right, Direction.Down) => Direction.Left,
      (Turn.Right, Direction.Right) => Direction.Down,
      (Turn.Right, Direction.Left) => Direction.Up,
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
  }

  [Flags]
  enum Edge { RightHandSide = 1, LeftHandSide = 2 }

  enum Turn
  {
    Left, Right
  }
}

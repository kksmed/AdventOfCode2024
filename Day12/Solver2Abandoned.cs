using System.Drawing;
using Common;

class Solver2Abandoned : Solver
{
  public override int Solve(char[,] data)
  {
    var costs = 0;
    var used = new bool[data.GetLength(0), data.GetLength(1)];
    foreach (var startPoint in GetUnused(used))
    {
      var plant = data[startPoint.X, startPoint.Y];
      var results = GetSidesAndArea(data, startPoint, plant, used, Edge.Behind, Direction.Right);

      var subCosts = results.Area * (1 + results.Sides);
      costs += subCosts;
      Console.WriteLine($"{plant}: {results.Area} x {1 + results.Sides} - total: {costs}");
    }

    return costs;
  }

  static (int Area, int Sides) GetSidesAndArea(
    char[,] map,
    Point point,
    char plant,
    bool[,] used,
    Edge edge,
    Direction direction
  )
  {
    const bool verbose = true;
    used[point.X, point.Y] = true;
    if (verbose)
      Console.WriteLine($"At: {point} - {direction} - edge: {edge} ");
    var area = 1;
    var sides = 0;

    var goLeft = NextStep(point, TurnDirection(direction, Turn.Left));
    var goStraight = NextStep(point, direction);
    var goRight = NextStep(point, TurnDirection(direction, Turn.Right));

    if (goLeft.CanGo)
    {
      if (!used[goLeft.Point.X, goLeft.Point.Y])
      {
        Edge newEdge = default;
        if (!goStraight.CanGo)
          newEdge = Edge.RightHandSide;
        var subResults = GetSidesAndArea(map, goLeft.Point, plant, used, newEdge, goLeft.Direction);
        sides += subResults.Sides;
        area += subResults.Area;
      }
    }
    else if (!edge.HasFlag(Edge.LeftHandSide))
    {
      sides++;
      if (verbose)
        Console.WriteLine($"+ new side to the left ({goLeft.Point})");
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
      else
      {
        // Check for double counting
        if (!goLeft.CanGo && !NextStep(goStraight.Point, TurnDirection(goStraight.Direction, Turn.Left)).CanGo)
        {
          sides--;
          if (verbose)
            Console.WriteLine($"- already counted: left side ({goLeft.Point})");
        }
        if (!goRight.CanGo && !NextStep(goStraight.Point, TurnDirection(goStraight.Direction, Turn.Right)).CanGo)
        {
          sides--;
          if (verbose)
            Console.WriteLine($"- already counted: right side ({goRight.Point})");
        }
      }
    }
    else
    {
      sides++;
      if (verbose)
        Console.WriteLine($"+ new side straight ({goStraight.Point})");
    }

    if (goRight.CanGo)
    {
      if (!used[goRight.Point.X, goRight.Point.Y])
      {
        Edge newEdge = default;
        if (!goStraight.CanGo)
          newEdge = Edge.LeftHandSide;
        if (edge.HasFlag(Edge.Behind))
          newEdge |= Edge.RightHandSide;
        var subResults = GetSidesAndArea(map, goRight.Point, plant, used, newEdge, goRight.Direction);
        sides += subResults.Sides;
        area += subResults.Area;
      }
      else
      {
        // Check for double counting
        if (!goStraight.CanGo && !NextStep(goRight.Point, TurnDirection(goRight.Direction, Turn.Left)).CanGo)
        {
          sides--;
          if (verbose)
            Console.WriteLine($"- already counted: left side ({goRight.Point})");
        }
      }
    }
    else if (!edge.HasFlag(Edge.RightHandSide))
    {
      sides++;
      if (verbose)
        Console.WriteLine($"+ new side to the right ({goRight.Point})");
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
      _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
    };
  }

  [Flags]
  enum Edge
  {
    RightHandSide = 1,
    LeftHandSide = 2,
    Behind = 4,
  }

  enum Turn
  {
    Left,
    Right,
  }
}

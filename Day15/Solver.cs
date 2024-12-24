using System.Drawing;

using Common;

namespace Day15;

class Solver : ISolver<(Point Start, Element[,] Warehouse, Direction[] Moves), int>
{
  public int Solve((Point Start, Element[,] Warehouse, Direction[] Moves) data)
  {
    Move(data.Start, data.Warehouse, data.Moves);
    return GetGps(data.Warehouse);
  }

  static void Move(Point robot, Element[,] warehouse, params Direction[] moves)
  {
    foreach (var move in moves)
    {
      // Print(warehouse);
      // Console.WriteLine($"Move: {ToChar(move)}");
      var newRobot = InnerMove(robot, move);
      if (newRobot == null)
      {
        // Console.WriteLine($"Can't move ({robot} {move})");
        continue;
      }

      // Console.WriteLine($"Moved ({robot}->{newRobot.Value} {move})");
      robot = newRobot.Value;
    }
    return;

    Point? InnerMove(Point p, Direction d)
    {
      var pToMove = d switch
      {
        Direction.Left => p with
        {
          X = p.X - 1
        },
        Direction.Right => p with
        {
          X = p.X + 1
        },
        Direction.Up => p with
        {
          Y = p.Y - 1
        },
        Direction.Down => p with
        {
          Y = p.Y + 1
        },
        _ => throw new InvalidOperationException()
      };
      if (pToMove.X < 0 || pToMove.Y < 0 || pToMove.X >= warehouse.GetLength(0) || pToMove.Y >= warehouse.GetLength(1))
        return null;

      var elementToMove = warehouse[p.X, p.Y];
      var elementInFront = warehouse[pToMove.X, pToMove.Y];
      switch (elementInFront)
      {
        case Element.Empty:
          break;
        case Element.Box:
          var moveBox = InnerMove(pToMove, d);
          if (moveBox == null)
            return null;
          break;
        case Element.Wall:
          return null;
        case Element.Robot:
          throw new InvalidOperationException("More robots?");
        default:
          throw new ArgumentOutOfRangeException();
      }
      warehouse[pToMove.X, pToMove.Y] = elementToMove;
      warehouse[p.X, p.Y] = Element.Empty;
      return pToMove;
    }
  }

  static int GetGps(Element[,] warehouse)
  {
    var gps = 0;
    for (var x = 0; x < warehouse.GetLength(0); x++)
    for (var y = 0; y < warehouse.GetLength(1); y++)
      if (warehouse[x, y] == Element.Box)
        gps += x + 1 + (y + 1) * 100;
    return gps;
  }
}
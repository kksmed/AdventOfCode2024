using System.Drawing;

using Common;

namespace Day15;

class Solver2 : ISolver<(Point Start, Element2[,] Warehouse, Direction[] Moves), int>
{
  public int Solve((Point Start, Element2[,] Warehouse, Direction[] Moves) data)
  {
    Move(data.Start, data.Warehouse, data.Moves);
    return GetGps(data.Warehouse);
  }

  static void Move(Point robot, Element2[,] warehouse, params Direction[] moves)
  {
    foreach (var move in moves)
    {
      var moveResult = InnerMove(robot, move);
      if (moveResult == null)
      {
        continue;
      }

      moveResult.Value.DoMove();
      robot = moveResult.Value.NewPosition;
    }
    return;

    (Point NewPosition, Action DoMove)? InnerMove(Point p, Direction d)
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
        throw new ArgumentOutOfRangeException(nameof(p));

      var elementToMove = warehouse[p.X, p.Y];
      var elementInFront = warehouse[pToMove.X, pToMove.Y];
      List<Action> moreMoves = new();
      switch (elementInFront)
      {
        case Element2.Empty:
          break;
        case Element2.BoxLeft:
        {
          var leftMoveBox = InnerMove(pToMove, d);
          if (leftMoveBox == null)
            return null;
          moreMoves.Add(leftMoveBox.Value.DoMove);
          if (d is Direction.Right or Direction.Left)
            break;
          var rightMoveBox = InnerMove(pToMove with
          {
            X = pToMove.X + 1
          }, d);
          if (rightMoveBox == null)
            return null;
          moreMoves.Add(leftMoveBox.Value.DoMove);
          moreMoves.Add(rightMoveBox.Value.DoMove);
          break;
        }
        case Element2.BoxRight:
        {
          var rightMoveBox = InnerMove(pToMove, d);
          if (rightMoveBox == null)
            return null;
          moreMoves.Add(rightMoveBox.Value.DoMove);
          if (d is Direction.Right or Direction.Left)
            break;
          var leftMoveBox = InnerMove(pToMove with
          {
            X = pToMove.X - 1
          }, d);
          if (leftMoveBox == null)
            return null;
          moreMoves.Add(leftMoveBox.Value.DoMove);
          break;
        }
        case Element2.Wall:
          return null;
        case Element2.Robot:
          throw new InvalidOperationException("More robots?");
        default:
          throw new ArgumentOutOfRangeException();
      }

      var doMove = () =>
        {
          foreach (var move in moreMoves)
            move();
          warehouse[pToMove.X, pToMove.Y] = elementToMove;
          warehouse[p.X, p.Y] = Element2.Empty;
        };
      return (pToMove, doMove);
    }
  }

  static int GetGps(Element2[,] warehouse)
  {
    var gps = 0;
    for (var x = 0; x < warehouse.GetLength(0); x++)
    for (var y = 0; y < warehouse.GetLength(1); y++)
      if (warehouse[x, y] == Element2.BoxLeft)
        gps += x + y * 100;
    return gps;
  }
}
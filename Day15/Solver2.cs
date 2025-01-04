using System.Drawing;
using Common;

namespace Day15;

class Solver2 : ISolver<Data2, int>
{
    public int Solve(Data2 data) => GetGps(Move(data.Start, data.Warehouse, false, data.Moves));

    public void Step((Point Start, Element2[,] Warehouse, Direction[] Moves) data)
    {
        Move(data.Start, data.Warehouse, true, data.Moves);
    }

    static Element2[,] Move(
        Point robot,
        Element2[,] warehouse,
        bool print,
        params Direction[] moves
    )
    {
        if (print)
        {
            Console.WriteLine($"Initial state:");
            Printer.Print(warehouse);
        }

        foreach (var move in moves)
        {
            if (print)
            {
                Console.WriteLine($"Move {Printer.ToChar(move)}:");
            }

            var newWarehouse = (Element2[,])warehouse.Clone();
            var moveResult = InnerMove(robot, move, newWarehouse);
            if (moveResult != null)
            {
                robot = moveResult.Value;
                warehouse = newWarehouse;
            }
            if (print)
            {
                Printer.Print(warehouse);
                Console.ReadKey();
            }
        }

        return warehouse;
    }

    static Point? InnerMove(Point p, Direction d, Element2[,] w)
    {
        var pToMove = d switch
        {
            Direction.Left => p with { X = p.X - 1 },
            Direction.Right => p with { X = p.X + 1 },
            Direction.Up => p with { Y = p.Y - 1 },
            Direction.Down => p with { Y = p.Y + 1 },
            _ => throw new InvalidOperationException(),
        };
        if (
            pToMove.X < 0
            || pToMove.Y < 0
            || pToMove.X >= w.GetLength(0)
            || pToMove.Y >= w.GetLength(1)
        )
            throw new ArgumentOutOfRangeException(nameof(p));

        var elementToMove = w[p.X, p.Y];
        var elementInFront = w[pToMove.X, pToMove.Y];
        switch (elementInFront)
        {
            case Element2.Empty:
                break;
            case Element2.BoxLeft:
            {
                var leftMoveBox = InnerMove(pToMove, d, w);
                if (leftMoveBox == null)
                    return null;
                if (d is Direction.Right or Direction.Left)
                    break;
                var rightMoveBox = InnerMove(pToMove with { X = pToMove.X + 1 }, d, w);
                if (rightMoveBox == null)
                    return null;
                break;
            }
            case Element2.BoxRight:
            {
                var rightMoveBox = InnerMove(pToMove, d, w);
                if (rightMoveBox == null)
                    return null;
                if (d is Direction.Right or Direction.Left)
                    break;
                var leftMoveBox = InnerMove(pToMove with { X = pToMove.X - 1 }, d, w);
                if (leftMoveBox == null)
                    return null;
                break;
            }
            case Element2.Wall:
                return null;
            case Element2.Robot:
                throw new InvalidOperationException("More robots?");
            default:
                throw new ArgumentOutOfRangeException();
        }

        w[pToMove.X, pToMove.Y] = elementToMove;
        w[p.X, p.Y] = Element2.Empty;

        return pToMove;
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

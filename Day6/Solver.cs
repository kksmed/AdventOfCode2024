using System.Drawing;
using Common;

namespace Day6;

public class Solver1 : ISolver<char[,], int>
{
    public virtual int Solve(char[,] data)
    {
        var position = FindStart(data);
        var direction = Direction.Up;
        var count = 0;
        var map = (char[,])data.Clone();
        while (map.InBounds(position))
        {
            var c = map[position.X, position.Y];

            switch (c)
            {
                case '^':
                case '.':
                    map[position.X, position.Y] = 'X';
                    count++;
                    position = Move(position, direction);
                    break;
                case 'X':
                    position = Move(position, direction);
                    break;
                case '#':
                    (position, direction) = ChangeDirection(position, direction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(map), c, "Unknown character");
            }
        }

        return count;
    }

    protected static Point Move(Point point, Direction direction)
    {
        return direction switch
        {
            Direction.Up => point with { Y = point.Y - 1 },
            Direction.Down => point with { Y = point.Y + 1 },
            Direction.Left => point with { X = point.X - 1 },
            Direction.Right => point with { X = point.X + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }

    protected static (Point Position, Direction Direction) ChangeDirection(
        Point position,
        Direction direction
    )
    {
        return direction switch
        {
            Direction.Up => Go(Direction.Down, Direction.Right),
            Direction.Down => Go(Direction.Up, Direction.Left),
            Direction.Left => Go(Direction.Right, Direction.Up),
            Direction.Right => Go(Direction.Left, Direction.Down),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };

        (Point Position, Direction Direction) Go(Direction d1, Direction d2) =>
            (Move(Move(position, d1), d2), d2);
    }

    protected static Point FindStart(char[,] data)
    {
        for (var x = 0; x < data.GetLength(0); x++)
        {
            for (var y = 0; y < data.GetLength(1); y++)
            {
                if (data[x, y] == '^')
                {
                    return new(x, y);
                }
            }
        }

        throw new ArgumentException("No start found", nameof(data));
    }

    protected enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}

public class Solver2 : Solver1
{
    public override int Solve(char[,] data)
    {
        var maxSteps = (data.Cast<char>().Count(x => x == '.') + 1) * 4;
        var start = FindStart(data);
        var count = 0;
        for (var x = 0; x < data.GetLength(0); x++)
        {
            for (var y = 0; y < data.GetLength(1); y++)
            {
                var place = data[x, y];
                if (place != '.')
                    continue;

                var map = (char[,])data.Clone();
                map[x, y] = '#';

                var steps = 0;
                var direction = Direction.Up;
                var position = start;
                while (data.InBounds(position) && steps < maxSteps)
                {
                    var p = map[position.X, position.Y];

                    switch (p)
                    {
                        case '^':
                        case '.':
                            steps++;
                            position = Move(position, direction);
                            break;
                        case '#':
                            (position, direction) = ChangeDirection(position, direction);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(data),
                                p,
                                "Unknown character"
                            );
                    }
                }

                if (steps == maxSteps)
                    count++;
            }
        }

        return count;
    }
}

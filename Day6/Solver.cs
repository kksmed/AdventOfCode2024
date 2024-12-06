using System.Drawing;

using Common;

namespace Day6;

public class Solver : ISolver<char[,]>
{
  public char[,] Parse(string[] input) => Parsing.ParseToCharMap(input);

  public int SolveFirst(char[,] data)
  {
    var position = FindStart(data);
    var direction = Direction.Up;
    var count = 0;
    while (InBounds(position, data))
    {
      var c = data[position.X, position.Y];

      switch (c)
      {
        case '^':
        case '.':
          data[position.X, position.Y] = 'X';
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
          throw new ArgumentOutOfRangeException(nameof(data), c, "Unknown character");
      }
    }

    return count;
  }

  static Point Move(Point point, Direction direction)
  {
    return direction switch
      {
        Direction.Up => point with { Y = point.Y - 1 },
        Direction.Down => point with { Y = point.Y + 1 },
        Direction.Left => point with { X = point.X - 1 },
        Direction.Right => point with { X = point.X + 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };
  }

  static (Point Position, Direction Direction) ChangeDirection(Point position, Direction direction)
  {
    return direction switch
      {
        Direction.Up => Go(Direction.Down, Direction.Right),
        Direction.Down => Go(Direction.Up, Direction.Left),
        Direction.Left => Go(Direction.Right, Direction.Up),
        Direction.Right => Go(Direction.Left, Direction.Down),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

    (Point Position, Direction Direction) Go(Direction d1, Direction d2) => (Move(Move(position, d1), d2), d2);
  }

  static bool InBounds(Point p, char[,] map) => p.X >= 0 && p.X < map.GetLength(0) && p.Y >= 0 && p.Y < map.GetLength(1);

  static Point FindStart(char[,] data)
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

  public int? SolveSecond(char[,] data)
  {
    return null;
  }
}

enum Direction
{
  Up,
  Down,
  Left,
  Right
}

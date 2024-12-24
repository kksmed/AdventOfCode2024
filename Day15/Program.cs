using System.Drawing;

using Common;

var example =
  """
  ##########
  #..O..O.O#
  #......O.#
  #.OO..O.O#
  #..O@..O.#
  #O#..O...#
  #O..O..O.#
  #.OO.O.OO#
  #....O...#
  ##########
  
  <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
  vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
  ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
  <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
  ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
  ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
  >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
  <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
  ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
  v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
  """;

Solving.Go1(example, new Parser(), new Solver());

class Parser : IParser<(Point Start, Element[,] Warehouse, Direction[] Moves)>
{
  public (Point Start, Element[,] Warehouse, Direction[] Moves) Parse(string[] input)
  {
    var map = new List<List<Element>>();
    var moves = new List<Direction>();
    var isMap = true;
    foreach (var line in input)
    {
      if (isMap)
      {
        if (line.Length == 0)
        {
          isMap = false;
          continue;
        }

        if (line.All(x => x == '#'))
          continue;

        map.Add(line.Where((_, n) => n > 0 && n < line.Length - 2).Select(x => x switch
        {
          '#' => Element.Wall,
          '.' => Element.Empty,
          'O' => Element.Box,
          '@' => Element.Robot,
          _ => throw new NotImplementedException()
        }).ToList());
      }
      else
      {
        moves.AddRange(line.Select(x => x switch
        {
          '^' => Direction.Up,
          '>' => Direction.Right,
          'v' => Direction.Down,
          '<' => Direction.Left,
          _ => throw new NotImplementedException()
        }));
      }
    }

    Point start = new(-1, -1);
    var warehouse = new Element[map[0].Count, map.Count];
    for (var y = 0; y < map.Count; y++)
    {
      var xRow = map[y];
      for (var x = 0; x < xRow.Count; x++)
      {
        var element = xRow[x];
        if (element == Element.Robot)
          start = new(x, y);
        warehouse[x, y] = element;
      }
    }
    return (start, warehouse, moves.ToArray());
  }
}

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
      switch (move)
      {
        case Direction.Up:
          break;
        case Direction.Right:
          break;
        case Direction.Down:
          break;
        case Direction.Left:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
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
enum Element
{
  Empty = 0,
  Box,
  Wall,
  Robot,
}

enum Direction
{
  Up,
  Right,
  Down,
  Left,
}
using System.Drawing;

using Common;

var example = 
  """
  ###############
  #.......#....E#
  #.#.###.#.###.#
  #.....#.#...#.#
  #.###.#####.#.#
  #.#.#.......#.#
  #.#.#####.###.#
  #...........#.#
  ###.#.#####.#.#
  #...#.....#.#.#
  #.#.#.###.#.#.#
  #.....#...#.#.#
  #.###.#.#.#.#.#
  #S..#.....#...#
  ###############
  """;

Solving.Go1(example, new Parser(), new Solver());

class Parser : IParser<(Node[,] Map, Point Start, Point End)>
{
  public (Node[,] Map, Point Start, Point End) Parse(string[] input)
  {
    Point start = new(-1, -1);
    Point end = new(-1, -1);
    var map = new Node[input[0].Length, input.Length];
    for (var y = 0; y < input.Length; y++)
    for (var x = 0; x < input[y].Length; x++)
    {
      var c = input[y][x];
      var node = new Node(new(x, y), c == '#');
      map[x, y] = node;
      if (c == 'S')
      {
        start = new(x, y);
        node.ShortestPaths[(int)Direction.East] = 0;
      }
      else if (c == 'E')
        end = new(x, y);
    }

    return (map, start, end);
  }
}

class Solver : ISolver<(Node[,] Map, Point Start, Point End), int>
{
  public int Solve((Node[,] Map, Point Start, Point End) data)
  {
    var unvisited = new List<Node>(data.Map.Cast<Node>().Where(x => !x.IsWall));

    while (unvisited.Count > 0)
    {
      var current = unvisited.OrderBy(x => x.ShortestPath).First();
      unvisited.Remove(current);
      current.IsVisited = true;

      if (current.P == data.End)
        return current.ShortestPath;

      foreach (var direction in Enum.GetValues<Direction>())
      {
        var next = Go(data.Map, current.P, direction);
        if (next.IsWall || next.IsVisited)
          continue;

        var newDistance = Math.Min(current.ShortestPath + 1000, current.ShortestPaths[(int)direction]) + 1;
        if (newDistance < next.ShortestPaths[(int)direction])
          next.ShortestPaths[(int)direction] = newDistance;
      }
    }

    throw new InvalidOperationException("No solution");
  }

  Node Go(Node[,] map, Point current, Direction direction)
  {
    Point next;
    switch (direction)
    {
      case Direction.North:
        next = current with
        {
          Y = current.Y - 1
        };
        break;
      case Direction.East:
        next = current with
        {
          X = current.X + 1
        };
        break;
      case Direction.South:
        next = current with
        {
          Y = current.Y + 1
        };
        break;
      case Direction.West:
        next = current with
        {
          X = current.X - 1
        };
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(direction));
    }
    return map[next.X, next.Y];
  }
}

record Node(Point P, bool IsWall = false)
{
  public int[] ShortestPaths { get; } = [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue];
  public int ShortestPath => ShortestPaths.Min();
  public bool IsVisited { get; set; } = IsWall;
}

enum Direction
{
  North = 0,
  East = 1,
  South = 2,
  West = 3
}   
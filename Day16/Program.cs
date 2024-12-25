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

var example2 =
  """
  #################
  #...#...#...#..E#
  #.#.#.#.#.#.#.#.#
  #.#.#.#...#...#.#
  #.#.#.#.###.#.#.#
  #...#.#.#.....#.#
  #.#.#.#.#.#####.#
  #.#...#.#.#.....#
  #.#.#####.#.###.#
  #.#.#.......#...#
  #.#.###.#####.###
  #.#.#...#.....#.#
  #.#.#.#####.###.#
  #.#.#.........#.#
  #.#.#.#########.#
  #S#.............#
  #################
  """;

Solving.Go(example2, new Parser(), new Solver2(), false);

// Solving.Go(example, new Parser(), new Solver1(), solverPart2: new Solver2());

class Parser : IParser<Data>
{
  public Data Parse(string[] input)
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

    return new(map, start, end);
  }
}

class Solver1 : ISolver<Data, int>
{
  public virtual int Solve(Data data)
  {
    var unvisited = new List<Node>(data.Map.Cast<Node>().Where(x => !x.IsWall));

    while (unvisited.Count > 0)
    {
      var current = unvisited.OrderBy(x => x.ShortestPath).First();
      unvisited.Remove(current);
      current.IsVisited = true;

      if (current.ShortestPath == int.MaxValue)
        break;

      foreach (var direction in Enum.GetValues<Direction>())
      {
        var next = Go(data.Map, current.P, direction);
        if (next.IsWall || next.IsVisited)
          continue;

        var newDistance = Math.Min(current.ShortestPath + 1000, current.ShortestPaths[(int)direction]) + 1;
        next.ShortestPaths[(int)direction] = newDistance;
      }
    }

    return data.Map[data.End.X, data.End.Y].ShortestPath;
  }

  protected static Node Go(Node[,] map, Point current, Direction direction)
  {
    var next = direction switch
    {
      Direction.North => current with
      {
        Y = current.Y - 1
      },
      Direction.East => current with
      {
        X = current.X + 1
      },
      Direction.South => current with
      {
        Y = current.Y + 1
      },
      Direction.West => current with
      {
        X = current.X - 1
      },
      _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
    return map[next.X, next.Y];
  }
}

class Solver2 : Solver1
{
  public override int Solve(Data data)
  {
    base.Solve(data);

    var partOfBestPath = new HashSet<Point>();

    var current = data.Map[data.End.X, data.End.Y];
    foreach (var pd in current.ShortestPaths.Select((x, d) => (Path: x, Direction: (Direction)d)).Where(x => x.Path == current.ShortestPath))
      BackTrack(current, pd.Direction);

    // foreach (var point in partOfBestPath)
    // {
    //   Console.WriteLine(point);
    // }
    return partOfBestPath.Count;

    void BackTrack(Node node, Direction direction)
    {
      partOfBestPath.Add(node.P);
      if (node.P == data.Start)
        return;

      var possibleDirections = new[]
        {
          direction, direction.Left(), direction.Right()
        }.Select(x => (Path: node.ShortestPaths[(int)x], Direction: x))
        .Select(x => x.Direction == direction || x.Path == int.MaxValue ? x : x with
        {
          Path = x.Path + 1000
        })
        .ToList();

      var shortestPath = possibleDirections.MinBy(x => x.Path).Path;
      foreach (var pd in possibleDirections.Where(x => x.Path == shortestPath))
      {
        var next = Go(data.Map, node.P, pd.Direction.Revert());
        if (partOfBestPath.Contains(next.P))
          continue;

        BackTrack(next, pd.Direction);
      }
    }
  }
}

record Data(Node[,] Map, Point Start, Point End);

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

static class DirectionExtensions
{
  public static Direction Revert(this Direction d) => d switch
  {
    Direction.North => Direction.South,
    Direction.East => Direction.West,
    Direction.South => Direction.North,
    Direction.West => Direction.East,
    _ => throw new ArgumentOutOfRangeException(nameof(d))
  };
  public static Direction Left(this Direction d) => d switch
  {
    Direction.North => Direction.West,
    Direction.East => Direction.North,
    Direction.South => Direction.East,
    Direction.West => Direction.South,
    _ => throw new ArgumentOutOfRangeException(nameof(d))
  };

  public static Direction Right(this Direction d) => d switch
  {
    Direction.North => Direction.East,
    Direction.East => Direction.South,
    Direction.South => Direction.West,
    Direction.West => Direction.North,
    _ => throw new ArgumentOutOfRangeException(nameof(d))
  };
}
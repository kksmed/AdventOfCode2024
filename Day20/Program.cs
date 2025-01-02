using System.Drawing;
using System.Security.Cryptography;

using Common;

var example =
  """
  ###############
  #...#...#.....#
  #.#.#.#.#.###.#
  #S#...#.#.#...#
  #######.#.#.###
  #######.#.#...#
  #######.#.###.#
  ###..E#...#...#
  ###.#######.###
  #...###...#...#
  #.#####.#.###.#
  #.#...#.#.#...#
  #.#.#.#.#.#.###
  #...#...#...###
  ###############
  """;

Console.WriteLine("Example");
Solving.Go(example, new Parser(), new Solver(1), false);
Console.WriteLine("Part 1");
Solving.Go(null, new Parser(), new Solver());

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
        node.ShortestPath = 0;
      }
      else if (c == 'E')
        end = new(x, y);
    }

    return new(map, start, end);
  }
}

class Solver(int MinimumGain = 100) : ISolver<Data, int>
{
  public int Solve(Data data) => FindCheat(data, MinimumGain).Count();

  static IEnumerable<(Point Start, Point End, int Distance)> FindCheat(Data data, int minimumGain)
  {
    var withoutCheats = GetShortestPath(data);
    Console.WriteLine($"Shortest without cheating: {withoutCheats}");

    var map = data.Map;
    ResetVisited(map);

    var unvisitedQueue = new Queue<Node>();
    unvisitedQueue.Enqueue(map.Get(data.Start));

    while (unvisitedQueue.Count > 0)
    {
      var current = unvisitedQueue.Dequeue();
      current.IsVisited = true;

      foreach (var next in current.P.GetNeighbours().Where(p => map.InBounds(p)).Select(p => map[p.X, p.Y]).Where(n => n is { IsVisited: false }))
      {
        if (!next.IsWall)
        {
          unvisitedQueue.Enqueue(next);
          continue;
        }

        // Cheat activated
        var distanceAfterWall = current.ShortestPath + 2;
        foreach (var afterCheat in next.P.GetNeighbours().Where(p => map.InBounds(p)).Select(p => map[p.X, p.Y]).Where(n => n is { IsVisited: false } && distanceAfterWall <= n.ShortestPath - minimumGain))
        {
          var cheatMap = map.Copy();
          cheatMap[afterCheat.P.X, afterCheat.P.Y].ShortestPath = distanceAfterWall;
          var newShortestDistance = CompleteShortestPath(cheatMap, unvisitedQueue.Select(x => x.P).Prepend(afterCheat.P), data.End);
          if (newShortestDistance <= withoutCheats - minimumGain)
          {
            Console.WriteLine($"Cheat found: 1: {next.P} 2: {afterCheat.P} new distance {newShortestDistance} ({withoutCheats - newShortestDistance})");
            yield return (next.P, afterCheat.P, newShortestDistance);
          }
        }
      }
    }
  }

  static void ResetVisited(Node[,] map)
  {
    foreach (var node in map)
    {
      node.IsVisited = false;
    }
  }

  static int GetShortestPath(Data data) => CompleteShortestPath(data.Map, [data.Start], data.End);

  static int CompleteShortestPath(Node[,] map, IEnumerable<Point> initialQueue, Point end)
  {
    var unvisitedQueue = new Queue<Node>(initialQueue.Select(map.Get));

    while (unvisitedQueue.Count > 0)
    {
      var current = unvisitedQueue.Dequeue();
      current.IsVisited = true;

      if (current.ShortestPath == int.MaxValue)
        throw new InvalidOperationException("Overflow or bug!");

      foreach (var next in current.P.GetNeighbours().Where(map.InBounds).Select(p => map[p.X, p.Y]).Where(n => n is { IsVisited: false, IsWall: false }))
      {
        next.ShortestPath = current.ShortestPath + 1;
        unvisitedQueue.Enqueue(next);
      }
    }

    return map.Get(end).ShortestPath;
  }
}

record Data(Node[,] Map, Point Start, Point End);

record Node(Point P, bool IsWall = false) : ICopyable<Node>
{
  public int ShortestPath { get; set; }
  public bool IsVisited { get; set; } = IsWall;

  public Node Copy() => new (P, IsWall);
}


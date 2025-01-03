using System.Diagnostics;
using System.Drawing;

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

Console.WriteLine("## Example");
Solving.Go(example, new Parser(), new Solver(1, 2), false);
Console.WriteLine("## Part 1");
Solving.Go(null, new Parser(), new Solver(MaxCheat:2));
Console.WriteLine("## Part 2");
// Solving.Go(example, new Parser(), new Solver(50, 20), false);
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

class Solver(int MinimumGain = 100, int MaxCheat = 20) : ISolver<Data, int>
{
  public int Solve(Data data) => CountCheats(data);

  int CountCheats(Data data)
  {
    var reverseMap = data.Map.Copy();
    reverseMap.Get(data.End).ShortestPath = 0;
    reverseMap.Get(data.Start).ShortestPath = int.MaxValue;
    var reverseDist = CompleteShortestPath(reverseMap, [data.End], data.Start);

    var withoutCheats = GetShortestPath(data);
    // Console.WriteLine($"Shortest without cheating: {withoutCheats}");
    if (reverseDist != withoutCheats)
      throw new InvalidOperationException($"Distances does not match: {reverseDist} vs {withoutCheats}");

    var cheats = new Dictionary<(Point Start, Point End), int>();
    var map = data.Map;
    ResetVisited(map);

    var unvisitedQueue = new Queue<Node>();
    unvisitedQueue.Enqueue(map.Get(data.Start));

    while (unvisitedQueue.Count > 0)
    {
      var current = unvisitedQueue.Dequeue();
      current.IsVisited = true;

      if (current.ShortestPath > withoutCheats - MinimumGain)
        break;

      foreach (var next in GetNeighbours(map, current).Where(n => n is { IsVisited: false }))
      {
        if (!next.IsWall)
        {
          unvisitedQueue.Enqueue(next);
          continue;
        }

        // We only consider each wall once
        //next.IsVisited = true;

        // Cheat activated
        //var cheatStart = next.P;
        foreach (var afterCheat in next.P.GetNeighbourhood(MaxCheat-1).Where(map.InBounds).Select(map.Get).Where(n => n is { IsVisited: false, IsWall: false }).Select(x => (Node: x, NewDistance: current.ShortestPath + 1 + Math.Abs(next.P.X - x.P.X) + Math.Abs(next.P.Y - x.P.Y))).Where(x => x.Node.ShortestPath >= x.NewDistance + MinimumGain))
        {
          var p = afterCheat.Node.P;
          // var cheatMap = map.Copy();
          // cheatMap[p.X, p.Y].ShortestPath = afterCheat.NewDistance;
          // var newShortestDistance = CompleteShortestPath(cheatMap, unvisitedQueue.Select(x => x.P).Prepend(p), data.End);
          var newShortestDistance = afterCheat.NewDistance + reverseMap.Get(p).ShortestPath;
          if (newShortestDistance <= withoutCheats - MinimumGain)
          {
            // Console.WriteLine($"Cheat found: {next.P} -> {p} new distance {newShortestDistance} ({withoutCheats - newShortestDistance})");

            var saved = withoutCheats - newShortestDistance;
            if (cheats.TryGetValue((Start: current.P, End: p), out var existingSave))
              saved = Math.Max(existingSave, saved);

            cheats[(Start: current.P, End: p)] = saved;
          }
        }
      }
    }

    // foreach (var group in cheats.Select(x => (ShortCut: x.Key, Saved: x.Value)).GroupBy(x => x.Saved).OrderBy(x =>x.Key))
    // {
    //   Console.WriteLine($"There are {group.Count()} cheats that save {group.Key} picoseconds.");
    //   if (group.Key == 76)
    //   {
    //     foreach (var c in group)
    //     {
    //       Console.WriteLine($"Cheat: {c.ShortCut}.");
    //     }
    //   }
    // }
    return cheats.Count;
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

      var newShortestPath = current.ShortestPath + 1;
      foreach (var next in GetNeighbours(map, current).Where(n => n is { IsVisited: false, IsWall: false }))
      {
        if (newShortestPath < next.ShortestPath)
        {
          next.ShortestPath = newShortestPath;
          unvisitedQueue.Enqueue(next);
        }
        else
        {
          next.IsVisited = true;
        }
      }
    }

    return map.Get(end).ShortestPath;
  }

  // Mostly, for simpler code
  static IEnumerable<Node> GetNeighbours(Node[,] map, Node n)
  {
    // Right
    if (n.P.X + 1 < map.GetLength(0))
      yield return map[n.P.X + 1, n.P.Y];

    // Up
    if (n.P.Y - 1 >= 0)
      yield return map[n.P.X, n.P.Y - 1];

    // Left
    if (n.P.X - 1 >= 0)
      yield return map[n.P.X - 1, n.P.Y];

    // Down
    if (n.P.Y + 1 < map.GetLength(1))
      yield return map[n.P.X, n.P.Y + 1];
  }
}

record Data(Node[,] Map, Point Start, Point End);

record Node(Point P, bool IsWall = false) : ICopyable<Node>
{
  public int ShortestPath { get; set; } = int.MaxValue;
  public bool IsVisited { get; set; } = IsWall;

  public Node Copy() => new (P, IsWall);
}


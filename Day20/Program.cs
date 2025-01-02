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

Solving.Go(example, new Parser(), new Solver(), false);

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

class Solver : ISolver<Data, int>
{

  public int Solve(Data data)
  {
    return GetShortestPath(data);
  }
  static int GetShortestPath(Data data)
  {
    var map = data.Map.Copy();
    var unvisitedQueue = new Queue<Node>();
    unvisitedQueue.Enqueue(map.Get(data.Start));

    while (unvisitedQueue.Count > 0)
    {
      var current = unvisitedQueue.Dequeue();
      current.IsVisited = true;

      if (current.ShortestPath == int.MaxValue)
        throw new InvalidOperationException("Overflow or bug!");

      foreach (var next in current.P.GetNeighbours().Where(p => map.InBounds(p)).Select(p => map[p.X, p.Y]).Where(n => n is { IsVisited: false, IsWall: false }))
      {
        next.ShortestPath = current.ShortestPath + 1;
        unvisitedQueue.Enqueue(next);
      }
    }

    return map.Get(data.End).ShortestPath;
  }
}

record Data(Node[,] Map, Point Start, Point End);

record Node(Point P, bool IsWall = false) : ICopyable<Node>
{
  public int ShortestPath { get; set; }
  public bool IsVisited { get; set; } = IsWall;

  public Node Copy() => new (P, IsWall);
}


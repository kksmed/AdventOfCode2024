using System.Drawing;

using Common;

var example =
  """
  5,4
  4,2
  4,5
  3,0
  2,1
  6,3
  2,4
  1,5
  0,6
  3,3
  2,6
  5,1
  1,2
  5,5
  2,5
  6,5
  1,4
  0,4
  6,4
  1,1
  6,1
  1,0
  0,5
  1,6
  2,0
  """;

Solving.Go(example, new Parser(), new Solver(7, 12), false);

Solving.Go(null, new Parser(), new Solver());

public class Parser : IParser<Point[]>
{
  public Point[] Parse(string[] input) => input.Select(x => x.Split(',').Select(int.Parse).ToList()).Select(x => new Point(x[0], x[1])).ToArray();
}

public class Solver(int Size = 71, int Bytes = 1024) : ISolver<Point[], int>
{
  public int Solve(Point[] data)
  {
    var distances = new int[Size, Size];
    for (var y = 0; y < Size; y++)
    for (var x = 0; x < Size; x++)
    {
      distances[x, y] = x == 0 && y == 0 ? 0 : int.MaxValue;
    }

    var walls = new HashSet<Point>();
    var visited = new HashSet<Point>();
    var unvisited = new List<Point>
    {
      new(0, 0)
    };

    while (unvisited.Count != 0)
    {
      var current = unvisited.Select(x => (Point: x, Distance: distances[x.X, x.Y])).First();
      if (current.Point == new Point(Size - 1, Size - 1))
        return current.Distance;

      unvisited.Remove(current.Point);
      visited.Add(current.Point);

      var nextDistance = current.Distance + 1;
      if (nextDistance > walls.Count)
      {
        walls.Add(data[current.Distance]);
      }

      var neighbors = GetNeighbors(current);
      foreach (var next in neighbors)
      {
        if (distances[next.X, next.Y] <= nextDistance)
          continue;
        distances[next.X, next.Y] = nextDistance;
        unvisited.Add(next);
      }
    }
    return -1;

    IEnumerable<Point> GetNeighbors((Point Point, int Distance) current) =>
      new[]
      {
        current.Point with
        {
          X = current.Point.X + 1
        },
        current.Point with
        {
          X = current.Point.X - 1
        },
        current.Point with
        {
          Y = current.Point.Y + 1
        },
        current.Point with
        {
          Y = current.Point.Y - 1
        }
      }.Where(p => p.X >= 0 && p.X < Size && p.Y >= 0 && p.Y < Size && !walls.Contains(p) && !visited.Contains(p));
  }
}

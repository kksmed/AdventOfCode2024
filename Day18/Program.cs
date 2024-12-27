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

class Parser : IParser<Point[]>
{
  public Point[] Parse(string[] input) => input.Select(x => x.Split(',').Select(int.Parse).ToList()).Select(x => new Point(x[0], x[1])).ToArray();
}

class Solver(int Size = 71, int Bytes = 1024) : ISolver<Point[], int>
{
  public int Solve(Point[] data)
  {
    var steps = new int[Size, Size];
    for (var y = 0; y < Size; y++)
    for (var x = 0; x < Size; x++)
    {
      steps[x, y] = x == 0 && y == 0 ? 0 : int.MaxValue;
    }

    var walls = new HashSet<Point>(data.Take(Bytes));
    var visited = new HashSet<Point>();
    List<Point> unvisited = [new(0, 0)];

    while (unvisited.Count != 0)
    {
      var current = unvisited.Select(x => (Point: x, Steps: steps[x.X, x.Y])).First();
      if (current.Point == new Point(Size - 1, Size - 1))
        return current.Steps;

      unvisited.Remove(current.Point);
      visited.Add(current.Point);

      var nextSteps = current.Steps + 1;
      // if (nextSteps > walls.Count)
      // {
      //   walls.Add(data[current.Steps]);
      // }

      var neighbors = GetNeighbors(current);
      foreach (var next in neighbors)
      {
        if (steps[next.X, next.Y] <= nextSteps)
          continue;
        steps[next.X, next.Y] = nextSteps;
        unvisited.Add(next);
      }
      // Printer.Print(steps, walls);
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

static class Printer
{
  public static void Print(int[,] steps, HashSet<Point> walls)
  {
    Console.Clear();
    for (var y = 0; y < steps.GetLength(1); y++)
    {
      for (var x = 0; x < steps.GetLength(0); x++)
      {
        if (walls.Contains(new(x, y)))
          Console.Write("#X");
        else if (steps[x, y] == int.MaxValue)
          Console.Write("  ");
        else
          Console.Write($"{steps[x, y]:X2}");
      }
      Console.WriteLine();
    }
    // Console.WriteLine(" < Press any key to continue >");
    // Console.ReadKey();
  }
}

using System.Drawing;
using System.Numerics;
using System.Text.RegularExpressions;

using Common;

var example =
  """
  p=0,4 v=3,-3
  p=6,3 v=-1,-3
  p=10,3 v=-1,2
  p=2,0 v=2,-1
  p=0,0 v=1,3
  p=3,0 v=-2,-2
  p=7,6 v=-1,-3
  p=3,0 v=-1,-2
  p=9,3 v=2,3
  p=7,3 v=-1,2
  p=2,4 v=2,-3
  p=9,5 v=-3,-3
  """;

Solving.Go1(example, new Parser(), new Solver(11, 7));
Solving.Go1(null, new Parser(), new Solver());

var keys = new Dictionary<char, int>
{
  ['a'] = -1000,
  ['s'] = -100,
  ['d'] = -10,
  ['f'] = -1,
  ['j'] = 1,
  ['k'] = 10,
  ['l'] = 100,
  [';'] = 1000,
};

var simulator = new Simulator(new Parser().Parse(File.ReadAllLines("input.txt")));
var steps = 0;
while (true)
{
  simulator.Print();

  Console.WriteLine($"Steps: {steps}");
  Console.Write("< Press key to continue: ");
  foreach (var key in keys)
  {
    Console.Write($"{key.Key} = {key.Value} ");
  }
  Console.WriteLine(">");
  var keyPress = Console.ReadKey().KeyChar;
  if (!keys.TryGetValue(keyPress, out var stepToTake))
    break;

  simulator.Step(stepToTake);
  steps += stepToTake;
}

public class Parser : IParser<IEnumerable<Robot>>
{
  static readonly Regex regex = new(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");
  public IEnumerable<Robot> Parse(string[] input) =>
    input.Select(x => regex.Match(x))
      .Select(x => new Robot(
        new(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)),
        new(int.Parse(x.Groups[3].Value), int.Parse(x.Groups[4].Value))));
}

public class Solver : ISolver<IEnumerable<Robot>, int>
{
  protected readonly int Wide;
  protected readonly int Tall;
  readonly int steps;
  public Solver(int wide = 101, int tall = 103, int steps = 100)
  {
    Wide = wide;
    Tall = tall;
    this.steps = steps;
  }

  public int Solve(IEnumerable<Robot> robots) => GetSafetyFactor(robots.Select(x => Step(MakePositive(x), steps)));

  Robot MakePositive(Robot robot)
  {
    if (robot.Velocity is { X: >= 0, Y: >= 0 })
      return robot;

    return robot with
    {
      Velocity = new(
        robot.Velocity.X >= 0 ? robot.Velocity.X : robot.Velocity.X % Wide + Wide,
        robot.Velocity.Y >= 0 ? robot.Velocity.Y : robot.Velocity.Y % Tall + Tall)
    };
  }

  protected Point Step(Robot robot, int seconds) =>
    new((robot.Position.X + robot.Velocity.X * seconds) % Wide, (robot.Position.Y + robot.Velocity.Y * seconds) % Tall);

  int GetSafetyFactor(IEnumerable<Point> endPositions)
  {
    int[] quadrant = [0, 0, 0, 0];
    foreach (var p in endPositions)
    {
      if (p.X < Wide / 2 && p.Y < Tall / 2)
      {
        quadrant[0]++;
      }
      else if (p.X > Wide / 2 && p.Y < Tall / 2)
      {
        quadrant[1]++;
      }
      else if (p.X < Wide / 2 && p.Y > Tall / 2)
      {
        quadrant[2]++;
      }
      else if (p.X > Wide / 2 && p.Y > Tall / 2)
      {
        quadrant[3]++;
      }
    }
    return quadrant[0] * quadrant[1] * quadrant[2] * quadrant[3];
  }
}

class Simulator(IEnumerable<Robot> robots) : Solver
{
  Robot[] Robots { get; } = robots.ToArray();

  public void Step(int n = 1)
  {
    foreach (var robot in Robots)
    {
      robot.Position = Step(robot, n);
    }
  }

  public void Print()
  {
    Console.Clear();
    Console.WriteLine(string.Join("", Enumerable.Range(0, Wide).Select(_ => "-")));
    for (var y = 0; y < Tall; y++)
    {
      Console.Write("|");
      for (var x = 0; x < Wide; x++)
      {
        if (Robots.Any(r => r.Position == new Point(x, y)))
        {
          Console.Write("#");
        }
        else
        {
          Console.Write(" ");
        }
      }
      Console.WriteLine("|");
    }
    Console.WriteLine(string.Join("", Enumerable.Range(0, Wide+2).Select(_ => "-")));
  }
}

public record Robot(Point StartPoint, Point Velocity)
{
  public Point Position { get; set; } = StartPoint;
}

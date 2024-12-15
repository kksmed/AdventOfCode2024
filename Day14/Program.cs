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

public class Parser : IParser<IEnumerable<Robot>>
{
  static readonly Regex regex = new(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");
  public IEnumerable<Robot> Parse(string[] input) =>
    input.Select(x => regex.Match(x))
      .Select(x => new Robot(
        new(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)),
        new(int.Parse(x.Groups[3].Value), int.Parse(x.Groups[4].Value))));
}

public class Solver(int Wide = 101, int Tall = 103, int Steps = 100) : ISolver<IEnumerable<Robot>, int>
{
  public int Solve(IEnumerable<Robot> robots) => GetSafetyFactor(robots.Select(x => Step(MakePositive(x), Steps)));

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

  Point Step(Robot robot, int seconds) =>
    new((robot.StartPoint.X + robot.Velocity.X * seconds) % Wide, (robot.StartPoint.Y + robot.Velocity.Y * seconds) % Tall);

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

public record Robot(Point StartPoint, Point Velocity);

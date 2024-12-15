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
  
Solving.Go(example, new Parser(), new Solver());

public class Parser : IParser<IEnumerable<Robot>>
{
  static readonly Regex regex = new(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");
  public IEnumerable<Robot> Parse(string[] input) =>
    input.Select(x => regex.Match(x))
      .Select(x => new Robot(
        new(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)),
        new(int.Parse(x.Groups[3].Value), int.Parse(x.Groups[4].Value))));

}

public record Robot(Point p, Vector2 v);
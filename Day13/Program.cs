using System.Drawing;
using System.Text.RegularExpressions;

using Common;

var example =
  """
  Button A: X+94, Y+34
  Button B: X+22, Y+67
  Prize: X=8400, Y=5400

  Button A: X+26, Y+66
  Button B: X+67, Y+21
  Prize: X=12748, Y=12176

  Button A: X+17, Y+86
  Button B: X+84, Y+37
  Prize: X=7870, Y=6450

  Button A: X+69, Y+23
  Button B: X+27, Y+71
  Prize: X=18641, Y=10279
  """;

Solving.Go(example, new Parser(), new Solver());

class Parser : IParser<IEnumerable<ClawMachine>>
{
  readonly Regex regexA = new("Button A: X\\+(\\d+), Y\\+(\\d+)");
  readonly Regex regexB = new("Button B: X\\+(\\d+), Y\\+(\\d+)");
  readonly Regex regexPrize = new("Prize: X=(\\d+), Y=(\\d+)");
  public IEnumerable<ClawMachine> Parse(string[] input)
  {
    for (var i = 0; i < input.Length; i++)
    {
      var line = input[i];
      if (string.IsNullOrWhiteSpace(line))
        continue;

      yield return new(ParsePoint(line, regexA), ParsePoint(input[++i], regexB), ParsePoint(input[++i], regexPrize));
    }
    yield break;

    Point ParsePoint(string line, Regex regex)
    {
      var match = regex.Match(line);
      return new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
    }
  }
}

class Solver : ISolver<IEnumerable<ClawMachine>, int>
{
  readonly int costOfA = 3;
  readonly int costOfB = 1;
  readonly int maxButtonPushes = 100;

  public int Solve(IEnumerable<ClawMachine> data) => data.Select(GetMinimumTokens).Sum();

  int GetMinimumTokens(ClawMachine arg)
  {
    var solutions = FindAllSolutions(arg);
    return solutions.Any() ? FindAllSolutions(arg).Min() : 0;
  }

  IEnumerable<int> FindAllSolutions(ClawMachine claw)
  {
    for (var a = 0; a <= maxButtonPushes && a * claw.ButtonA.X <= claw.Prize.X && a * claw.ButtonA.Y <= claw.Prize.Y; a++)
    for (var b = 0; b <= maxButtonPushes && a * claw.ButtonA.X + b * claw.ButtonB.X <= claw.Prize.X && a * claw.ButtonA.Y + b * claw.ButtonB.Y <= claw.Prize.Y; b++)
      if (a * claw.ButtonA.X + b * claw.ButtonB.X == claw.Prize.X && a * claw.ButtonA.Y + b * claw.ButtonB.Y == claw.Prize.Y)
        yield return a * costOfA + b * costOfB;
  }
}
record ClawMachine(Point ButtonA, Point ButtonB, Point Prize);
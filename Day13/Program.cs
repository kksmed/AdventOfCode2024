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

//Solving.Go(example, new Parser(), new Solver(), solverPart2: new Solver2());
Solving.Go(example, new Parser(), new Solver2());

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

class Solver2 : ISolver<IEnumerable<ClawMachine>, long>
{
  readonly long costOfA = 3;
  readonly long costOfB = 1;
  const long offSet = 10000000000000;

  public long Solve(IEnumerable<ClawMachine> data) => data.Select(FindMinimum).Sum();

  long FindMinimum(ClawMachine claw)
  {
    var cm = new CorrectedClawMachine(claw);

    // Test
    var maxAPushes = Math.Min(cm.Prize.X / cm.ButtonA.X, cm.Prize.Y / cm.ButtonA.Y);
    var maxBPushes = Math.Min(cm.Prize.X / cm.ButtonB.X, cm.Prize.Y / cm.ButtonB.Y);
    if (maxAPushes * cm.ButtonA.X + maxBPushes * cm.ButtonB.X < cm.Prize.X || maxAPushes * cm.ButtonA.Y + maxBPushes * cm.ButtonB.Y < cm.Prize.Y)
    {
      Console.WriteLine($"{claw} A: {maxAPushes} B: {maxBPushes} - quick fail 1");
      return 0;
    }

    var gradeA = (double)cm.ButtonA.X / cm.ButtonA.Y;
    var gradeB = (double)cm.ButtonB.X / cm.ButtonB.Y;
    var prizeGrade = (double)cm.Prize.X / cm.Prize.Y;
    if (gradeA < prizeGrade && gradeB < prizeGrade || gradeA > prizeGrade && gradeB > prizeGrade)
    {
      Console.WriteLine($"{claw} A: {maxAPushes} B: {maxBPushes} - quick fail 2");
      return 0;
    }

    if ((cm.ButtonA.X + cm.ButtonA.Y) / (double)costOfA > (cm.ButtonB.X + cm.ButtonB.Y) / (double)costOfB)
    {
      for (var a = maxAPushes; a >= 0; a--)
      {
        var rest = (X: cm.Prize.X - a * cm.ButtonA.X, Y: cm.Prize.Y - a * cm.ButtonA.Y);
        var b = rest.X / cm.ButtonB.X;
        if (rest.X % cm.ButtonB.X == 0 && b * cm.ButtonB.Y == rest.Y)
        {
          Console.WriteLine($"{claw} A: {a} B: {b}");
          return a * costOfA + b * costOfB;
        }
      }
    }
    else
    {
      for (var b = maxBPushes; b >= 0; b--)
      {
        var rest = (X: cm.Prize.X - b * cm.ButtonB.X, Y: cm.Prize.Y - b * cm.ButtonB.Y);
        var a = rest.X / cm.ButtonA.X;
        if (rest.X % cm.ButtonA.X == 0 && a * cm.ButtonA.Y == rest.Y)
        {
          Console.WriteLine($"{claw} A: {a} B: {b}");
          return a * costOfA + b * costOfB;
        }
      }
    }

    return 0;
  }

  record CorrectedClawMachine(Point ButtonA, Point ButtonB, (long X, long Y) Prize)
  {
    public CorrectedClawMachine(ClawMachine claw) : this(claw.ButtonA, claw.ButtonB, (claw.Prize.X + offSet, claw.Prize.Y + offSet)) { }
  }
}

record ClawMachine(Point ButtonA, Point ButtonB, Point Prize);
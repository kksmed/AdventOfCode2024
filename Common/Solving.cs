using System.Diagnostics;

namespace Common;

public static class Solving
{
  public static void Go<TData, T1Result, T2Result>(
    string? example,
    IParser<TData> parser,
    ISolver<TData, T1Result>? solverPart1 = null,
    ISolver<TData, T2Result>? solverPart2 = null)
  {
    var sw = Stopwatch.StartNew();
    var definingType = (solverPart1 as object ?? parser).GetType();
    Console.WriteLine($"### {definingType.Namespace ?? definingType.Assembly.GetName().Name} ###");
    Console.WriteLine("## Part 1 ##");

    TData? parsedExample = default;
    if (example != null)
    {
      var exampleInput = example.Split(Environment.NewLine);

      parsedExample = parser.Parse(exampleInput);

      if (solverPart1 == null)
      {
        Console.WriteLine($"Parsed example: {parsedExample}");
        return;
      }

      var exampleAnswerPart1 = solverPart1.Solve(parsedExample);
      Console.WriteLine($"# Example (1): {exampleAnswerPart1}");
    }

    var input = File.ReadAllLines("input.txt");
    var parsedInput = parser.Parse(input);
    var answerPart1 = solverPart1.Solve(parsedInput);
    Console.WriteLine($"# Answer (1): {answerPart1}");

    if (solverPart2 == null)
      return;

    Console.WriteLine("");
    Console.WriteLine("## Part 2 ##");

    if (example == null && parsedExample != null)
    {
      var exampleAnswerPart2 = solverPart2.Solve(parsedExample);
      Console.WriteLine($"# Example (2): {exampleAnswerPart2}");
    }

    var answerPart2 = solverPart2.Solve(parsedInput);
    Console.WriteLine($"# Answer (2): {answerPart2}");

    Console.WriteLine("");
    Console.WriteLine($"Evaluated in: {sw.Elapsed}");
  }
}

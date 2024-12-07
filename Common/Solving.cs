namespace Common;

public static class Solving
{
  public static void Go<T1Data, T1Result>(
    string example,
    IParser<T1Data> parser,
    ISolver<T1Data, T1Result>? part1Solver = null,
    ISolver<T1Data, T1Result>? part2Solver = null) where T1Data : class =>
    DoAll(example, parser, part1Solver, null, part2Solver);

  public static void Go<T1Data, T1Result, T2Data, T2Result>(
    string example,
    IParser<T1Data> parserPart1,
    ISolver<T1Data, T1Result> solverPart1,
    IParser<T2Data> parserPart2,
    ISolver<T2Data, T2Result>? solverPart2 = null) where T2Data : class =>
    DoAll(example, parserPart1, solverPart1, parserPart2, solverPart2);

  static void DoAll<T1Data, T1Result, T2Data, T2Result>(
    string example,
    IParser<T1Data> parserPart1,
    ISolver<T1Data, T1Result>? solverPart1 = null,
    IParser<T2Data>? parserPart2 = null,
    ISolver<T2Data, T2Result>? solverPart2 = null) where T2Data : class
  {
    Console.WriteLine($"### Day: {(solverPart1 as object ?? parserPart1).GetType().Namespace} ###");
    Console.WriteLine("## Part 1 ##");
    var exampleInput = example.Split(Environment.NewLine);
    var exampleDataPart1 = parserPart1.Parse(exampleInput);

    if (solverPart1 == null)
    {
      Console.WriteLine($"Parsed example: {exampleDataPart1}");
      return;
    }

    var exampleAnswerPart1 = solverPart1.Solve(exampleDataPart1);
    Console.WriteLine($"# Example: {exampleAnswerPart1}");

    var input = File.ReadAllLines("input.txt");
    var dataPart1 = parserPart1.Parse(input);
    var answerPart1 = solverPart1.Solve(dataPart1);
    Console.WriteLine($"# Answer (1): {answerPart1}");

    if (parserPart2 == null && solverPart2 == null)
      return;

    Console.WriteLine("");
    Console.WriteLine("## Part 2 ##");

    T2Data exampleDataPart2, dataPart2;
    if (parserPart2 == null)
    {
      exampleDataPart2 = exampleDataPart1 as T2Data
        ?? throw new InvalidOperationException("Parser for part 2 is missing and data type differs from part 1");
      dataPart2 = dataPart1 as T2Data
        ?? throw new InvalidOperationException("Parser for part 2 is missing and data type differs from part 1");

      // Should be redundant, but type system needs it
      if (solverPart2 == null) return;
    }
    else
    {
      exampleDataPart2 = parserPart2.Parse(exampleInput);
      if (solverPart2 == null)
      {
        Console.WriteLine($"Parsed example: {exampleDataPart2}");
        return;
      }

      dataPart2 = parserPart2.Parse(input);
    }

    var exampleAnswerPart2 = solverPart2.Solve(exampleDataPart2);
    Console.WriteLine($"# Example: {exampleAnswerPart2}");

    var answerPart2 = solverPart2.Solve(dataPart2);
    Console.WriteLine($"# Answer (2): {answerPart2}");
  }
}

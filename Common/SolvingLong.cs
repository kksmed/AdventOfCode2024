namespace Common;

public static class SolvingLong
{
  public static long SolveExample<T>(ISolverLong<T> solver, string example)
  {
    Console.WriteLine("Example:");
    return SolveFirstThenSecond(solver, example.Split(Environment.NewLine));
  }

  public static long Solve<T>(ISolverLong<T> solver)
  {
    Console.WriteLine("Real input:");
    return SolveFirstThenSecond(solver, File.ReadAllLines("input.txt"));
  }

  static long SolveFirstThenSecond<T>(ISolverLong<T> solver, string[] input)
  {
    Console.WriteLine($"Day: {solver.GetType().Namespace}");

    var data = solver.Parse(input);
    var firstAnswer = solver.SolveFirst(data);
    Console.WriteLine($"1st answer: {firstAnswer}");

    var secondAnswer = solver.SolveSecond(data);
    if (secondAnswer is null)
      return firstAnswer;

    Console.WriteLine($"2nd answer: {secondAnswer}");
    return secondAnswer.Value;
  }
}

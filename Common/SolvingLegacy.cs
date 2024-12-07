namespace Common;

public static class SolvingLegacy
{
  public static int SolveExample<T>(ISolverLegacy<T> solver, string example)
  {
    Console.WriteLine("Example:");
    return SolveFirstThenSecond(solver, example.Split(Environment.NewLine));
  }

  public static int Solve<T>(ISolverLegacy<T> solver)
  {
    Console.WriteLine("Real input:");
    return SolveFirstThenSecond(solver, File.ReadAllLines("input.txt"));
  }

  static int SolveFirstThenSecond<T>(ISolverLegacy<T> solver, string[] input)
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
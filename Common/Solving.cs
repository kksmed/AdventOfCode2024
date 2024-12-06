namespace Common;

public static class Solving
{
  public static int SolveExample<T>(ISolver<T> solver, string example)
  {
    Console.WriteLine("Example:");
    return SolveFirstThenSecond(solver, example.Split(Environment.NewLine));
  }

  public static int Solve<T>(ISolver<T> solver)
  {
    Console.WriteLine("Real input:");
    return SolveFirstThenSecond(solver, File.ReadAllLines("input.txt"));
  }

  static int SolveFirstThenSecond<T>(ISolver<T> solver, string[] input)
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

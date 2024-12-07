using Common;

namespace Day7;

public class Solver : ISolverLong<IEnumerable<(long TestValue, List<long> Numbers)>>
{
  public IEnumerable<(long TestValue, List<long> Numbers)> Parse(string[] input)
  {
    return input.Select(
      x =>
        {
          var parts = x.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
          return (long.Parse(parts[0]),
            parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList());
        });
  }

  public long SolveFirst(IEnumerable<(long TestValue, List<long> Numbers)> data) =>
    data.Where(IsPossible).Select(x => x.TestValue).Sum();

  static bool IsPossible((long TestValue, List<long> Numbers) arg) =>
    IsPossible(arg.Numbers[0], arg.TestValue, arg.Numbers.Skip(1).ToList());

  static bool IsPossible(long currentValue, long testValue, List<long> numbers)
  {
    if (currentValue > testValue) return false;

    if (numbers.Count == 0)
      return currentValue == testValue;

    var n = numbers[0];
    var rest = numbers.Skip(1).ToList();
    return IsPossible(currentValue + n, testValue, rest) || IsPossible(currentValue * n, testValue, rest);
  }

  public long? SolveSecond(IEnumerable<(long TestValue, List<long> Numbers)> data)
  {
    return null;
  }
}

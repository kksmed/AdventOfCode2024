using Common;

namespace Day7;

public class Solver2 : ISolver<IEnumerable<(long TestValue, List<long> Numbers)>, long>
{
  public long Solve(IEnumerable<(long TestValue, List<long> Numbers)> data) =>
    data.Where(IsPossible2).Select(x => x.TestValue).Sum();

  static bool IsPossible2((long TestValue, List<long> Numbers) arg) =>
    IsPossible2(arg.Numbers[0], arg.TestValue, arg.Numbers.Skip(1).ToList());

  static bool IsPossible2(long currentValue, long testValue, List<long> numbers)
  {
    if (currentValue > testValue) return false;

    if (numbers.Count == 0)
      return currentValue == testValue;

    var n = numbers[0];
    var rest = numbers.Skip(1).ToList();
    return IsPossible2(currentValue + n, testValue, rest)
      || IsPossible2(currentValue * n, testValue, rest)
      || IsPossible2(long.Parse($"{currentValue}{n}"), testValue, rest);
  }
}

using Common;

namespace Day7;

public class Parser : IParser<IEnumerable<(long TestValue, List<long> Numbers)>>
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
}

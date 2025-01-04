using System.Text.RegularExpressions;
using Common;

namespace Day3;

public partial class Solver2
    : IParser<IEnumerable<IInstruction>>,
        ISolver<IEnumerable<IInstruction>, int>
{
    readonly Regex regex = MyRegex();

    public IEnumerable<IInstruction> Parse(string[] input) =>
        input
            .SelectMany(x => regex.Matches(x))
            .Select(match =>
                match.Groups[0].Value switch
                {
                    "do()" => (IInstruction)new Do(),
                    "don't()" => new Dont(),
                    var mul when mul.StartsWith("mul", StringComparison.InvariantCulture) =>
                        new Mul(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)),
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(input),
                        $"Unknown instruction: {match.Groups[0].Value}"
                    ),
                }
            );

    public int Solve(IEnumerable<IInstruction> data)
    {
        var sum = 0;
        var doMul = true;
        foreach (var instruction in data)
        {
            switch (instruction)
            {
                case Do _:
                    doMul = true;
                    break;
                case Dont _:
                    doMul = false;
                    break;
                case Mul mul when doMul:
                    sum += mul.A * mul.B;
                    break;
                case Mul when !doMul:
                    continue;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(data),
                        $"Unknown instruction: {instruction}"
                    );
            }
        }
        return sum;
    }

    [GeneratedRegex(@"(mul\((\d+),(\d+)\))|(do\(\))|(don't\(\))")]
    private static partial Regex MyRegex();
}

public interface IInstruction;

record Mul(int A, int B) : IInstruction;

record Do : IInstruction;

record Dont : IInstruction;

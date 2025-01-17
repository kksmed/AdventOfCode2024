using Common;

var smallExample = """
  x00: 1
  x01: 1
  x02: 1
  y00: 0
  y01: 1
  y02: 0

  x00 AND y00 -> z00
  x01 XOR y01 -> z01
  x02 OR y02 -> z02
  """;

var bigExample = """
  x00: 1
  x01: 0
  x02: 1
  x03: 1
  x04: 0
  y00: 1
  y01: 1
  y02: 1
  y03: 1
  y04: 1

  ntg XOR fgs -> mjb
  y02 OR x01 -> tnw
  kwq OR kpj -> z05
  x00 OR x03 -> fst
  tgd XOR rvg -> z01
  vdt OR tnw -> bfw
  bfw AND frj -> z10
  ffh OR nrd -> bqk
  y00 AND y03 -> djm
  y03 OR y00 -> psh
  bqk OR frj -> z08
  tnw OR fst -> frj
  gnj AND tgd -> z11
  bfw XOR mjb -> z00
  x03 OR x00 -> vdt
  gnj AND wpb -> z02
  x04 AND y00 -> kjc
  djm OR pbm -> qhw
  nrd AND vdt -> hwm
  kjc AND fst -> rvg
  y04 OR y02 -> fgs
  y01 AND x02 -> pbm
  ntg OR kjc -> kwq
  psh XOR fgs -> tgd
  qhw XOR tgd -> z09
  pbm OR djm -> kpj
  x03 XOR y03 -> ffh
  x00 XOR y04 -> ntg
  bfw OR bqk -> z06
  nrd XOR fgs -> wpb
  frj XOR qhw -> z04
  bqk OR frj -> z07
  y03 OR x01 -> nrd
  hwm AND bqk -> z03
  tgd XOR rvg -> z12
  tnw OR pbm -> gnj
  """;

var parser = new Parser();
var solver = new Solver();
Solving.Go(smallExample, parser, solver, false);
Solving.Go(bigExample, parser, solver);

class Parser : IParser<Data>
{
  public Data Parse(string[] input)
  {
    var values = new Dictionary<string, bool>();
    var instructions = new List<Instruction>();
    var isInstruction = false;
    foreach (var line in input)
    {
      if (!isInstruction)
      {
        if (line.Length == 0)
        {
          isInstruction = true;
          continue;
        }
        var parts = line.Split(":");
        var key = parts[0].Trim();
        var value = parts[1].Trim() switch
        {
          "0" => false,
          "1" => true,
          _ => throw new InvalidOperationException($"Invalid value, {parts[1]}, for "),
        };
        values[key] = value;
        continue;
      }

      var p = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
      if (p is not [_, _, _, "->", _])
        throw new InvalidOperationException("Invalid input: " + line);

      var operation = p[1] switch
      {
        "AND" => Operation.And,
        "OR" => Operation.Or,
        "XOR" => Operation.Xor,
        _ => throw new InvalidOperationException("Invalid operation"),
      };
      instructions.Add(
        new()
        {
          Left = p[0],
          Right = p[2],
          Output = p[4],
          Operation = operation,
        }
      );
    }
    return new() { Values = values, Instructions = instructions };
  }
}

class Solver : ISolver<Data, long>
{
  public long Solve(Data data)
  {
    var values = data.Values;
    foreach (var instruction in data.Instructions)
    {
      var left = values[instruction.Left];
      var right = values[instruction.Right];
      var result = instruction.Operation switch
      {
        Operation.And => left & right,
        Operation.Or => left | right,
        Operation.Xor => left ^ right,
        _ => throw new InvalidOperationException("Invalid operation"),
      };
      values[instruction.Output] = result;
    }

    var solution = 0L;
    foreach (var kv in values.Where(x => x.Key.StartsWith('z')).OrderByDescending(x => x.Key))
    {
      solution <<= 1;
      solution += kv.Value ? 1 : 0;
    }

    return solution;
  }
}

record Data
{
  public Dictionary<string, bool> Values { get; init; }
  public List<Instruction> Instructions { get; init; }
}

record Instruction
{
  public string Left { get; init; }
  public string Right { get; init; }
  public string Output { get; init; }
  public Operation Operation { get; init; }
}

enum Operation
{
  And,
  Or,
  Xor,
}

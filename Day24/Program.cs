using System.Diagnostics;
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
Solving.Go(null, parser, new Validator());

class Parser : IParser<Data>
{
  public Data Parse(string[] input)
  {
    var values = new Dictionary<string, bool>();
    var instructions = new List<Gate>();
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
    return new() { Values = values, Gates = instructions.ToArray() };
  }
}

class Solver : ISolver<Data, long>
{
  public long Solve(Data data)
  {
    var values = data.Values;
    var instructions = new Queue<Gate>(data.Gates);
    var staleCount = 0;
    while (instructions.Count > 0)
    {
      var instruction = instructions.Dequeue();
      if (!values.TryGetValue(instruction.Left, out var left) || !values.TryGetValue(instruction.Right, out var right))
      {
        staleCount++;
        instructions.Enqueue(instruction);

        if (staleCount > instructions.Count)
          return -1;

        continue;
      }

      staleCount = 0;
      var result = instruction.Operation switch
      {
        Operation.And => left & right,
        Operation.Or => left | right,
        Operation.Xor => left ^ right,
        _ => throw new InvalidOperationException("Invalid operation"),
      };
      values[instruction.Output] = result;
    }

    return Combiner.GetValue('z', values);
  }
}

class Solver2 : ISolver<Data, string>
{
  readonly Solver solver1 = new();

  public string Solve(Data data)
  {
    var x = Combiner.GetValue('x', data.Values);
    var y = Combiner.GetValue('y', data.Values);
    var expectedSum = x + y;
    Console.WriteLine($"x: {x} y: {y} expected sum:{expectedSum})");

    var unchangedData = data.Copy();
    var unchanged = solver1.Solve(unchangedData);
    Console.WriteLine($"Unchanged output: {unchanged}");

    var diff = expectedSum ^ unchanged;
    Console.WriteLine($"Diff: {diff:B}");

    var diffBits = new List<int>();
    for (var i = 0; i < 64; i++)
    {
      if ((diff & 1L << i) != 0)
        diffBits.Add(i);
    }
    Console.WriteLine($"Bits: {string.Join(',', diffBits)}");

    var outputBits = GetOutputs('z', unchangedData.Values);
    var wrongOutputs = diffBits.Select(i => outputBits[i]).ToList();
    Console.WriteLine($"Outputs: {string.Join(',', wrongOutputs)}");

    var wrongGates = GetWrongGates(data.Gates, wrongOutputs).Distinct().ToList();
    Console.WriteLine($"Wrong gates: {wrongGates.Count}");
    Console.WriteLine($"{string.Join(Environment.NewLine, wrongGates.Select(g => g.ToString()))}");

    // var checkedSwaps = new HashSet<string>();
    var total = 0L;
    var batch = 0L;
    var sw = Stopwatch.StartNew();
    foreach (var swap in GenerateSwaps(data, 4))
    {
      var output = solver1.Solve(swap.Data);
      if (output == expectedSum)
        return swap.Swaps;
      var swapId = string.Join(',', swap.swapIds.Select(p => $"{p.Item1}<>{p.Item2}"));
      // if (!checkedSwaps.Add(swapId))
      //   throw new InvalidOperationException($"Duplicate swap: {swapId}");
      batch++;
      total++;
      if (sw.ElapsedMilliseconds > 1000)
      {
        // Console.WriteLine($"Checked swaps: {checkedSwaps.Count} - {count} in {sw.Elapsed}");
        Console.WriteLine($"Checked swaps: {batch}/{total} in {sw.Elapsed}");
        Console.WriteLine($"Avg iterations / ms: {batch / sw.ElapsedMilliseconds}");
        Console.WriteLine($"Most recent swap: ({swapId}) {swap.Swaps}, Output: {output} (not {expectedSum})");
        sw.Restart();
        batch = 0;
      }
    }
    throw new InvalidOperationException("No solution found");
  }

  static List<Gate> GetWrongGates(Gate[] dataInstructions, List<string> wrongOutputs)
  {
    var queue = new Queue<string>(wrongOutputs);
    var list = new List<Gate>();
    while (queue.Count > 0)
    {
      var output = queue.Dequeue();

      var instruction = dataInstructions.FirstOrDefault(x => x.Output == output);

      if (instruction == null)
        continue;

      list.Add(instruction);
      queue.Enqueue(instruction.Left);
      queue.Enqueue(instruction.Right);
    }

    return list;
  }

  static IEnumerable<(Data Data, string Swaps, List<(int, int)> swapIds)> GenerateSwaps(Data data, int swapCount)
  {
    foreach (var swaps in GetPairs(data.Gates.Length, 0, swapCount))
    {
      var dataCopy = data.Copy();
      foreach (var (j, k) in swaps)
        (dataCopy.Gates[j].Output, dataCopy.Gates[k].Output) = (dataCopy.Gates[k].Output, dataCopy.Gates[j].Output);

      var swapString = string.Join(
        ",",
        swaps.SelectMany(x => new[] { data.Gates[x.Item1].Output, data.Gates[x.Item2].Output }).Order()
      );

      yield return (dataCopy, swapString, swaps);
    }

    yield break;

    static IEnumerable<List<(int, int)>> GetPairs(int length, int start, int pairCount)
    {
      if (pairCount == 0)
        throw new ArgumentException(" must be greater than 0", nameof(pairCount));

      for (var j = start; j < length; j++)
      {
        for (var k = j + 1; k < length; k++)
        {
          List<(int, int)> pairs = [(j, k)];
          if (pairCount == 1)
          {
            yield return pairs;
          }
          else
          {
            foreach (var restPairs in GetPairs(length, k + 1, pairCount - 1))
            {
              if (restPairs.Count != pairCount - 1)
                continue;

              yield return pairs.Concat(restPairs).ToList();
            }
          }
        }
      }
    }
  }

  static List<string> GetOutputs(char prefix, Dictionary<string, bool> values) =>
    values.Where(x => x.Key.StartsWith(prefix)).OrderByDescending(x => x.Key).Select(x => x.Key).ToList();
}

class Validator : ISolver<Data, string>
{
  public string Solve(Data data)
  {
    (string, string)[] swaps = [("z10", "ggn"), ("ndw", "jcb"), ("grm", "z32"), ("z39", "twr")];

    foreach (var (outA, outB) in swaps)
    {
      Swap(data.Gates, outA, outB);
    }

    var carryIn = ""; // No carry in for the first gate
    for (var i = 0; i <= 44; i++) // 44 is from looking manually at the input
    {
      Console.WriteLine($"Checking {i}...");
      var xGate = $"x{i:D2}";
      var yGate = $"y{i:D2}";
      var and = GetGate(data.Gates, xGate, yGate, Operation.And);
      var xor = GetGate(data.Gates, xGate, yGate, Operation.Xor);

      if (i == 0)
      {
        if (xor.Output != "z00")
          throw new InvalidOperationException(
            $"XOR gate for {xGate} and {yGate} should output z00, but it outputs {xor.Output}"
          );

        carryIn = and.Output;
        continue;
      }

      var xorToOut = GetGate(data.Gates, xor.Output, carryIn, Operation.Xor);
      if (xorToOut.Output != $"z{i:D2}")
        throw new InvalidOperationException(
          $"XOR gate for {xor.Output} and {carryIn} should output z{i:D2}, but it outputs {xorToOut.Output}"
        );

      var andToCarry = GetGate(data.Gates, xor.Output, carryIn, Operation.And);
      var orToCarry = GetGate(data.Gates, andToCarry.Output, and.Output, Operation.Or);
      carryIn = orToCarry.Output;
    }

    // Comfirm by running the gates
    var xValue = Combiner.GetValue('x', data.Values);
    var yValue = Combiner.GetValue('y', data.Values);
    var expectedSum = xValue + yValue;
    Console.WriteLine($"x: {xValue} y: {yValue} expected sum:{expectedSum})");

    var output = new Solver().Solve(data);
    Console.WriteLine($"Output: {output}");

    if (expectedSum != output)
      throw new InvalidOperationException($"Expected sum: {expectedSum}, but got {output}");

    return string.Join(",", swaps.SelectMany(x => new[] { x.Item1, x.Item2 }).Order());
  }

  static Gate GetGate(Gate[] gates, string inA, string inB, Operation op)
  {
    var gate = gates.SingleOrDefault(x =>
      x.Operation == op && (x.Left == inA && x.Right == inB || x.Left == inB && x.Right == inA)
    );

    if (gate == null)
      throw new InvalidOperationException($"{op}-gate not found for {inA} and {inB}");

    return gate;
  }

  static Gate GetGate(Gate[] gates, string output)
  {
    var gate = gates.SingleOrDefault(x => x.Output == output);
    if (gate == null)
      throw new InvalidOperationException($"Instruction outputting {output} not found");
    return gate;
  }

  static void Swap(Gate[] instructions, string a, string b)
  {
    var instructionA = GetGate(instructions, a);
    var instructionB = GetGate(instructions, b);

    instructionB.Output = a;
    instructionA.Output = b;
  }
}

static class Combiner
{
  public static long GetValue(char prefix, Dictionary<string, bool> values)
  {
    var bitCounter = 0;
    var value = 0L;
    foreach (var kv in values.Where(x => x.Key.StartsWith(prefix)).OrderByDescending(x => x.Key))
    {
      bitCounter++;
      value <<= 1;
      value += kv.Value ? 1 : 0;

      if (bitCounter > 63)
        throw new InvalidOperationException("Overflow!");
    }
    return value;
  }
}

record Data
{
  public required Dictionary<string, bool> Values { get; init; }
  public required Gate[] Gates { get; init; }

  public Data Copy() => new() { Values = new(Values), Gates = Gates.ToArray() };
}

record Gate
{
  public required string Left { get; init; }
  public required string Right { get; init; }
  public required string Output { get; set; }
  public required Operation Operation { get; init; }
}

enum Operation
{
  And,
  Or,
  Xor,
}

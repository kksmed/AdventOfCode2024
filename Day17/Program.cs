using System.Text.RegularExpressions;

using Common;

using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

var example =
  """
  Register A: 729
  Register B: 0
  Register C: 0
  
  Program: 0,1,5,4,3,0
  """;

Solving.Go1(example, new Parser(), new Interpreter());
Console.WriteLine("### Part 2 ###");
Solving.Go1(null, new Parser(), new Solver());

class Parser : IParser<Input>
{
  readonly Regex regexA = new(@"Register A: (\d+)");
  readonly Regex regexB = new(@"Register B: (\d+)");
  readonly Regex regexC = new(@"Register C: (\d+)");
  readonly Regex regexProgram = new("Program: ([0-7]),([0-7])(,([0-7]),([0-7]))*");

  public Input Parse(string[] input)
  {
    var a = int.Parse(regexA.Match(input[0]).Groups[1].Value);
    var b = int.Parse(regexB.Match(input[1]).Groups[1].Value);
    var c = int.Parse(regexC.Match(input[2]).Groups[1].Value);
    // Expecting input[3] to be empty
    var match = regexProgram.Match(input[4]);
    List<(Instruction Instruction, ComboOperand ComboOperand)> program =
    [
      (Instruction: (Instruction)int.Parse(match.Groups[1].Value), ComboOperand: (ComboOperand)int.Parse(match.Groups[2].Value))
    ];
    for (var i = 0; i < match.Groups[3].Captures.Count; i++)
    {
      program.Add((Instruction: (Instruction)int.Parse(match.Groups[4].Captures[i].Value), ComboOperand: (ComboOperand)int.Parse(match.Groups[5].Captures[i].Value)));
    }
    return new (a, b, c, program.ToArray());
  }
}

class Interpreter : ISolver<Input, string>
{
  public string Solve(Input data) => string.Join(", ", Execute(data));

  public static IEnumerable<int> Execute(Input data)
  {
    var a = data.A;
    var b = data.B;
    var c = data.C;
    var pc = 0;
    while (pc < data.Program.Length)
    {
      var (instruction, comboOperand) = data.Program[pc];
      switch (instruction)
      {
        case Instruction.Adv:
          a >>= GetIntOperandValue(comboOperand);
          TestForNegative(a);
          break;
        case Instruction.Bxl:
          b ^= (long)comboOperand;
          TestForNegative(b);
          break;
        case Instruction.Bst:
          b = GetOperandValue(comboOperand) % 8;
          Console.WriteLine($"B: {b}");
          TestForNegative(b);
          break;
        case Instruction.Jnz:
          if (a != 0)
          {
            if ((int)comboOperand % 2 == 1)
              throw new InvalidOperationException("Odd jumping!");
            pc = (int)comboOperand >> 1;
            TestForNegative(pc);
            continue;
          }
          break;
        case Instruction.Bxc:
          b ^= c;
          TestForNegative(b);
          break;
        case Instruction.Out:
          var o = GetOperandValue(comboOperand) % 8;
          TestForNegative(o);
          yield return (int)o;
          break;
        case Instruction.Bdv:
          b = a >> GetIntOperandValue(comboOperand);
          TestForNegative(b);
          break;
        case Instruction.Cdv:
          c = a >> GetIntOperandValue(comboOperand);
          Console.WriteLine($"C: {c%8}");
          TestForNegative(c);
          break;
        default:
          throw new InvalidOperationException("Invalid instruction");
      }

      pc++;
    }

    yield break;

    int GetIntOperandValue(ComboOperand comboOperand)
    {
      var value = GetOperandValue(comboOperand);
      if (value > int.MaxValue)
        throw new InvalidOperationException("Too big int value");
      return (int)value;
    }

    long GetOperandValue(ComboOperand comboOperand) => comboOperand switch
    {
      ComboOperand.Zero => 0,
      ComboOperand.One => 1,
      ComboOperand.Two => 2,
      ComboOperand.Three => 3,
      ComboOperand.A => a,
      ComboOperand.B => b,
      ComboOperand.C => c,
      ComboOperand.Reserved => throw new InvalidOperationException("Reserved operand"),
      _ => throw new ArgumentOutOfRangeException(nameof(comboOperand), comboOperand, "Invalid operand")
    };

    void TestForNegative(long value)
    {
      if (value < 0)
        throw new InvalidOperationException("Negative value");
    }
  }
}

class Solver : ISolver<Input, long>
{
  public long Solve(Input data)
  {
    var wantedOutput = data.Program.SelectMany(x => new[]
    {
      (int)x.Instruction, (int)x.ComboOperand
    }).ToArray();

    var solutions = FindA(wantedOutput, []);
    if (solutions.Count == 0)
    {
      Console.WriteLine("No solution found.");
      return -1;
    }
    Console.WriteLine($"Amount of solutions: {solutions.Count}");

    var solution = solutions.Order().First();
    Console.WriteLine("Smallest solution: " + solution);

    // var test = Interpreter.Execute(data with { A = solution }).ToList();
    // if (test.SequenceEqual(wantedOutput))
    //   Console.WriteLine("Verified.");
    // else
    // {
    //   Console.WriteLine("Failed verification.");
    //   Console.WriteLine("Expected: " + string.Join(", ", wantedOutput));
    //   Console.WriteLine("Actual: " + string.Join(", ", test));
    // }

    return solution;
  }

  static List<long> FindA(int[] wantedOutputs, List<(int Value, int Shift)> reserved, long candidate = 0, int output = 0)
  {
    if (output == wantedOutputs.Length)
    {
      return [candidate];
    }

    List<long> newSolutions = [];
    var literal1 = wantedOutputs[3]; // 5
    var literal2 = wantedOutputs[7]; // 6
    var wantedOutput = wantedOutputs[output];
    for(var cShift = 0; cShift < 8; cShift++)
    {
      var b = cShift ^ literal1;
      var c = cShift ^ literal2 ^ wantedOutput;
      var newValues = (long)c << cShift | (uint)b;
      if (cShift < 3 && (newValues % 8 != b || (newValues >> cShift) % 8 != c))
      {
        continue;
      }

      var shift = output * 3;
      newValues <<= shift;
      if (newValues < 0)
        throw new InvalidOperationException("Too large value");

      var newCandidate = candidate | newValues;
      if ((newCandidate >> shift) % 8 != b || (newCandidate >> shift + cShift) % 8 != c || reserved.Any(x => (newCandidate >> x.Shift) % 8 != x.Value))
      {
        continue;
      }

      var newReserved = reserved.Where(x => x.Shift > shift).ToList();
      newReserved.Add((Value: c, Shift: shift + cShift));

      var solutions = FindA(wantedOutputs, newReserved, newCandidate, output + 1);
      solutions = solutions.Where(x => (x >> shift) % 8 == b && (x >> shift + cShift) % 8 == c).ToList();
      newSolutions.AddRange(solutions);
    }

    return newSolutions;
  }
}

enum ComboOperand
{
  Zero= 0,
  One = 1,
  Two = 2,
  Three = 3,
  A = 4,
  B = 5,
  C = 6,
  Reserved = 7
}

enum Instruction
{
  Adv = 0,
  Bxl = 1,
  Bst = 2,
  Jnz = 3,
  Bxc = 4,
  Out = 5,
  Bdv = 6,
  Cdv = 7,
}

record Input(long A, long B, long C, (Instruction Instruction, ComboOperand ComboOperand)[] Program);



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
  public string Solve(Input data) => string.Join(",",Execute(data));

  static IEnumerable<int> Execute(Input data)
  {
    var a =data.A;
    var b = data.B;
    var c = data.C;
    var pc = 0;
    while (pc < data.Program.Length)
    {
      var (instruction, comboOperand) = data.Program[pc];
      switch (instruction)
      {
        case Instruction.Adv:
          a >>= GetOperandValue(comboOperand);
          break;
        case Instruction.Bxl:
          b ^= (int)comboOperand;
          break;
        case Instruction.Bst:
          b = GetOperandValue(comboOperand) % 8;
          break;
        case Instruction.Jnz:
          if (a != 0)
          {
            if ((int)comboOperand % 2 == 1)
              throw new InvalidOperationException("Odd jumping!");
            pc = (int)comboOperand >> 1;
            continue;
          }
          break;
        case Instruction.Bxc:
          b ^= c;
          break;
        case Instruction.Out:
          yield return GetOperandValue(comboOperand) % 8;
          break;
        case Instruction.Bdv:
          b = a >> GetOperandValue(comboOperand);
          break;
        case Instruction.Cdv:
          c = a >> GetOperandValue(comboOperand);
          break;
        default:
          throw new InvalidOperationException("Invalid instruction");
      }

      pc++;
    }

    yield break;

    int GetOperandValue(ComboOperand comboOperand) => comboOperand switch
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

record Input(int A, int B, int C, (Instruction Instruction, ComboOperand ComboOperand)[] Program);



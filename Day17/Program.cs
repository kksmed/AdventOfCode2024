﻿using System.Text.RegularExpressions;

using Common;

var example =
  """
  Register A: 729
  Register B: 0
  Register C: 0
  
  Program: 0,1,5,4,3,0
  """;

Solving.GoParse(example, new Parser());

class Parser : IParser<Input>
{
  Regex regexA = new(@"Register A: (\d+)");
  Regex regexB = new(@"Register B: (\d+)");
  Regex regexC = new(@"Register C: (\d+)");
  Regex regexProgram = new("Program: ([0-7]),([0-7])(,([0-7]),([0-7]))*");
  public Input Parse(string[] input)
  {
    var a = int.Parse(regexA.Match(input[0]).Groups[1].Value);
    var b = int.Parse(regexB.Match(input[1]).Groups[1].Value);
    var c = int.Parse(regexC.Match(input[2]).Groups[1].Value);
    // Expecting input[3] to be empty
    var match = regexProgram.Match(input[4]);
    (Instruction Instruction, ComboOperand ComboOperand)[] program = [];
    return new (a, b, c, program);
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

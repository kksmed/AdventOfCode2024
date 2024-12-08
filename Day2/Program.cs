using Common;

using Day2;

var example =
  """
  7 6 4 2 1
  1 2 7 8 9
  9 7 6 2 1
  1 3 2 4 5
  8 6 4 4 1
  1 3 6 7 9
  """;

var parser = new Parser();
var solver = new Solver();
var data = parser.Parse(File.ReadAllLines("input.txt"));
foreach (var record in data)
{
  if (SolverPart2.IsSafe2(record) != Solver2Part2.IsSafe2(record))
  {
    Console.WriteLine("Different results");
    Console.WriteLine(string.Join(" ", record));
    Console.WriteLine(SolverPart2.IsSafe2(record));
    Console.WriteLine(Solver2Part2.IsSafe2(record));
  }
}

Console.WriteLine("");
Console.WriteLine("#### First solver ####");
Solving.Go(example, new Parser(), new Solver(), new SolverPart2());

Console.WriteLine("");
Console.WriteLine("#### Second solver ####");

Solving.Go(example, new Parser(), new Solver2Part1(), new Solver2Part2());

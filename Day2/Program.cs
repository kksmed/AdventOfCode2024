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

// var solver = new Solver();
// var data = solver.Parse(File.ReadAllLines("input.txt"));
// foreach (var record in data)
// {
//   if (Solver.IsSafe2(record) != Solver2.IsSafe2(record))
//   {
//     Console.WriteLine("Different results");
//     Console.WriteLine(string.Join(" ", record));
//     Console.WriteLine(Solver.IsSafe2(record));
//     Console.WriteLine(Solver2.IsSafe2(record));
//   }
// }
// Console.WriteLine("## First solver ##");
// var solver = new Solver();
// Solving.SolveExample(solver, example);
// Solving.Solve(solver);
//
Console.WriteLine("## Second solver ##");
var solver2 = new Solver2();
Solving.SolveExample(solver2, example);
Solving.Solve(solver2);
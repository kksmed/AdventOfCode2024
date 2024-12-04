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

Console.WriteLine("## First solver ##");
var solver = new Solver();
Solving.SolveExample(solver, example);
Solving.Solve(solver);

Console.WriteLine("## Second solver ##");
var solver2 = new Solver2();
Solving.SolveExample(solver2, example);
Solving.Solve(solver2);
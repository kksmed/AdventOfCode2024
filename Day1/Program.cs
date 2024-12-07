using Common;

using Day1;

var example =
  """
  3   4
  4   3
  2   5
  1   3
  3   9
  3   3
  """;

var solver = new Solver();

SolvingLegacy.SolveExample(solver, example);

SolvingLegacy.Solve(solver);

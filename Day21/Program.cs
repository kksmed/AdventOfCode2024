using Common;
using Day21;

var example = """
  029A
  980A
  179A
  456A
  379A
  """;

StringSolver.InitializeCache();
Solving.Go(example, new NoParser(), new StringSolver(2));
Solving.Go(null, new NoParser(), new StringSolver(25));

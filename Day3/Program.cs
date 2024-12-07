using Common;

using Day3;

var example = """
              xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
              """;

Solver solver = new();
SolvingLegacy.SolveExample(solver, example);
SolvingLegacy.Solve(solver);

Console.WriteLine("### PART 2 ###");
Solver2 solver2 = new();
SolvingLegacy.SolveExample(solver2, "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))");
SolvingLegacy.Solve(solver2);

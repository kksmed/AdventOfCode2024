using Common;

using Day4;

var example =
  """
  MMMSXXMASM
  MSAMXMSMSA
  AMXSXMAAMM
  MSAMASMSMX
  XMASAMXAMM
  XXAMMXXAMA
  SMSMSASXSS
  SAXAMASAAA
  MAMMMXMMMM
  MXMXAXMASX
  """;

Solver solver = new();
SolvingLegacy.SolveExample(solver, example);
SolvingLegacy.Solve(solver);
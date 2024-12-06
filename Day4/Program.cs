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
Solving.SolveExample(solver, example);
Solving.Solve(solver);
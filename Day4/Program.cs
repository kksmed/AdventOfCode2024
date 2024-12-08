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

Solving.Go(example, new CharMapParser(), new Solver1(), new Solver2());

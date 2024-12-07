﻿using Common;

using Day6;

var example =
  """
  ....#.....
  .........#
  ..........
  ..#.......
  .......#..
  ..........
  .#..^.....
  ........#.
  #.........
  ......#...
  """;

Solver solver = new();
SolvingLegacy.SolveExample(solver, example);
SolvingLegacy.Solve(solver);

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
Solving.SolveExample(solver, example);
Solving.Solve(solver);

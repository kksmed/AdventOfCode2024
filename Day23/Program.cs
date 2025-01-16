using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Common;

var example = """
  kh-tc
  qp-kh
  de-cg
  ka-co
  yn-aq
  qp-ub
  cg-tb
  vc-aq
  tb-ka
  wh-tc
  yn-cg
  kh-ub
  ta-co
  de-co
  tc-td
  tb-wq
  wh-td
  ta-ka
  td-qp
  aq-cg
  wq-ub
  ub-vc
  de-ta
  wq-aq
  wq-vc
  wh-yn
  ka-de
  kh-ta
  co-tc
  wh-qp
  tb-vc
  td-yn
  """;

// Solving.Go(example, new Parser(), new Solver(), new Solver2());

Solving.Go(null, new Parser(), new Solver2());

class Parser : IParser<Dictionary<string, HashSet<string>>>
{
  public Dictionary<string, HashSet<string>> Parse(string[] input)
  {
    Dictionary<string, HashSet<string>> connections = new();
    foreach (var s in input)
    {
      var parts = s.Split('-');
      var from = parts[0];
      var to = parts[1];

      AddToConnections(from, to);
      // We add both ways as connections are bidirectional
      AddToConnections(to, from);
    }

    return new(connections);

    void AddToConnections(string from, string to)
    {
      var existingConnections = connections.GetValueOrDefault(from, []);
      existingConnections.Add(to);
      connections[from] = existingConnections;
    }
  }
}

class Solver : ISolver<Dictionary<string, HashSet<string>>, int>
{
  public int Solve(Dictionary<string, HashSet<string>> data)
  {
    var groups = LanFinder.GetGroups(data, 3).ToList();
    // foreach (var group in groups)
    // {
    //   Console.WriteLine(string.Join(",", group));
    // }
    return groups.Count(x => x.Any(c => c.StartsWith('t')));
  }
}

class Solver2 : ISolver<Dictionary<string, HashSet<string>>, string>
{
  public string Solve(Dictionary<string, HashSet<string>> data)
  {
    for (var i = data.Count; i >= 3; i--)
    {
      if (LanFinder.TryGetGroup(data, i, out var group))
      {
        Console.WriteLine($"Group found at size: {i}");

        return string.Join(',', group.Order());
      }

      Console.WriteLine($"Not groups at size: {i}");
    }
    throw new InvalidOperationException("No solution found");
  }
}

static class LanFinder
{
  public static bool TryGetGroup(
    Dictionary<string, HashSet<string>> connections,
    int groupSize,
    [NotNullWhen(true)] out HashSet<string>? group
  )
  {
    Dictionary<string, HashSet<string>> connectionsToRemove;
    do
    {
      connectionsToRemove = connections.Where(x => x.Value.Count < groupSize - 1).ToDictionary();
      var filteredConnections = connections
        .Where(x => !connectionsToRemove.ContainsKey(x.Key))
        .ToDictionary(x => x.Key, x => x.Value.Where(y => !connectionsToRemove.ContainsKey(y)).ToHashSet());
      Console.WriteLine($"Connections removed: {connectionsToRemove.Count}");
      connections = filteredConnections;
    } while (connectionsToRemove.Count > 0);

    var alreadyChecked = new HashSet<string>();
    var largestGroup = 0;
    foreach (var kvConnections in connections)
    {
      var sw = Stopwatch.StartNew();
      alreadyChecked.Add(kvConnections.Key);
      if (TryGetSubGroup([kvConnections.Key], alreadyChecked.ToHashSet(), out group))
        return true;
      Console.WriteLine($"Checked : {kvConnections.Key} in {sw.Elapsed}");
    }

    group = null;
    return false;

    bool TryGetSubGroup(
      HashSet<string> group,
      HashSet<string> alreadyCheckedWithinGroup,
      [NotNullWhen(true)] out HashSet<string>? subGroup
    )
    {
      if (group.Count > largestGroup)
      {
        largestGroup = group.Count;
        Console.WriteLine($"Largest group: {largestGroup}");
      }
      if (groupSize == group.Count)
      {
        subGroup = group;
        return true;
      }

      var last = group.Last();
      var lastConnections = connections[last];

      foreach (var newCandidate in lastConnections.Where(x => !alreadyCheckedWithinGroup.Contains(x)))
      {
        var newConnections = connections[newCandidate];
        if (newConnections.Count < groupSize - 1 || !group.All(newConnections.Contains))
        {
          alreadyCheckedWithinGroup.Add(newCandidate);
          continue;
        }

        group.Add(newCandidate);
        alreadyCheckedWithinGroup.Add(newCandidate);
        if (TryGetSubGroup(group, alreadyCheckedWithinGroup, out subGroup))
          return true;
        group.Remove(newCandidate);
        alreadyCheckedWithinGroup.Remove(newCandidate);
      }

      subGroup = null;
      return false;
    }
  }

  public static List<HashSet<string>> GetGroups(Dictionary<string, HashSet<string>> connections, int groupSize)
  {
    List<HashSet<string>> groups = [];
    foreach (var kvConnections in connections.Where(x => x.Value.Count >= groupSize - 1))
    {
      groups.AddRange(FindSubGroups([kvConnections.Key], connections, groupSize));
    }

    return groups.Select(x => string.Join(",", x.Order())).Distinct().Select(x => x.Split(",").ToHashSet()).ToList();

    static List<HashSet<string>> FindSubGroups(
      HashSet<string> group,
      Dictionary<string, HashSet<string>> connections,
      int groupSize
    )
    {
      if (groupSize == group.Count)
        return [group];

      List<HashSet<string>> subGroups = [];
      var last = group.Last();
      var lastConnections = connections[last];
      foreach (
        var newGroup in lastConnections
          .Where(newCandidate => !group.Contains(newCandidate))
          .Select(newCandidate => new { newCandidate, otherConnections = connections[newCandidate] })
          .Where(x => x.otherConnections.Count >= groupSize - 1 && group.All(x.otherConnections.Contains))
          .Select(x => group.Append(x.newCandidate).ToHashSet())
      )
      {
        subGroups.AddRange(FindSubGroups(newGroup, connections, groupSize));
      }

      return subGroups;
    }
  }
}

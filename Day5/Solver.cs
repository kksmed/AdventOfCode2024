using Common;

namespace Day5;

class Solver : ISolver<(List<PageOrderingRule> Rules, List<int[]> Pages)>
{
  public (List<PageOrderingRule> Rules, List<int[]> Pages) Parse(string[] input)
  {
    var rules = new List<PageOrderingRule>();
    var pages = new List<int[]>();

    var isRules = true;
    foreach (var line in input)
    {
      if (string.IsNullOrWhiteSpace(line))
      {
        isRules = false;
        continue;
      }

      if (isRules)
      {
        var parts = line.Split('|');
        rules.Add(new(int.Parse(parts[0]), int.Parse(parts[1])));
      }
      else
      {
        pages.Add(line.Split(',').Select(int.Parse).ToArray());
      }
    }

    return (rules, pages);
  }

  public int SolveFirst((List<PageOrderingRule> Rules, List<int[]> Pages) data) =>
    data.Pages.Where(x => IsCorrectOrder(data.Rules, x)).Select(Middle).Sum();

  static bool IsCorrectOrder(List<PageOrderingRule> rules, int[] pages)
  {
    for (var i = 1; i < pages.Length; i++)
    {
      var page = pages[i];
      var rulesForPage = rules.Where(r => r.Before == page).ToList();
      if (rulesForPage.Select(r => Array.FindIndex(pages, 0, i, p => p == r.After)).Any(j => j > -1))
      {
        return false;
      }
    }

    return true;
  }

  static int Middle(int[] list) => list[list.Length / 2];

  public int? SolveSecond((List<PageOrderingRule> Rules, List<int[]> Pages) data) =>
    data.Pages.Where(x => !IsCorrectOrder(data.Rules, x)).Select(x => FixOrder(data.Rules, x)).Select(Middle).Sum();

  static int[] FixOrder(List<PageOrderingRule> rules, int[] pages) =>
    pages.OrderBy(p => p, new PageComparer(rules)).ToArray();
}

class PageComparer(List<PageOrderingRule> rules) : IComparer<int>
{
  public int Compare(int x, int y)
  {
    var rule = rules.FirstOrDefault(r => r.Before == x && r.After == y || r.Before == y && r.After == x);
    return rule is null ? 0 : rule.Before == x ? -1 : 1;
  }
}

record PageOrderingRule(int Before, int After);
using System.Diagnostics.CodeAnalysis;

namespace Common;

public interface ISolver<in TData, out TResult>
{
  [return: NotNull]
  TResult Solve(TData data);
}

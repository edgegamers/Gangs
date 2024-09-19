using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace Mock;

public class MockStatManager : IStatManager {
  public IEnumerable<IStat> Stats { get; } = [];
}
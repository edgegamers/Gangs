using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace Mock;

public class MockStatManager : IStatManager {
  public IList<IStat> Stats { get; } = [];
}
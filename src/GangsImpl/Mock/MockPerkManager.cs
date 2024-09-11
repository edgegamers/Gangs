using GangsAPI.Perks;
using GangsAPI.Services;

namespace Mock;

public class MockPerkManager : IPerkManager {
  public IEnumerable<IPerk> Perks { get; } = [];
}
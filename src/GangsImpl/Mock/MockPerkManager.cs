using GangsAPI.Perks;
using GangsAPI.Services;

namespace Mock;

public class MockPerkManager : IPerkManager {
  public List<IPerk> Perks { get; set; } = [];
}
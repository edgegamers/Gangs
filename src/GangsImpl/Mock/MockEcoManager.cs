using GangsAPI.Data;
using GangsAPI.Services;

namespace Mock;

public class MockEcoManager : IEcoManager {
  public Task<bool> CanAfford(PlayerWrapper player, int cost,
    bool excludeGangCredits = false) {
    throw new NotImplementedException();
  }

  public Task<int> TryPurchase(PlayerWrapper player, int balanceDue,
    bool print = true, string? item = null, bool excludeGangCredits = false) {
    throw new NotImplementedException();
  }

  public Task<int> Grant(PlayerWrapper player, int amount, bool print = true,
    string? reason = null) {
    throw new NotImplementedException();
  }

  public Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null) {
    throw new NotImplementedException();
  }

  public Task<int> Grant(ulong player, int amount, bool print = true,
    string? reason = null) {
    throw new NotImplementedException();
  }
}
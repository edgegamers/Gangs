using GangsAPI.Data;
using GangsAPI.Services;

namespace Mock;

public class MockEcoManager : IEcoManager {
  public Task<bool> CanAfford(PlayerWrapper player, int cost,
    bool excludeGangCredits = false) {
    return Task.FromResult(true);
  }

  public Task<int> GetBalance(PlayerWrapper player,
    bool excludeGangCredits = false) {
    return Task.FromResult(0);
  }

  public Task<int> TryPurchase(PlayerWrapper player, int balanceDue,
    bool print = true, string? item = null, bool excludeGangCredits = false) {
    return Task.FromResult(0);
  }

  public Task<int> Grant(PlayerWrapper player, int amount, bool print = true,
    string? reason = null) {
    return Task.FromResult(amount);
  }

  public Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null) {
    return Task.FromResult(amount);
  }

  public Task<int> Grant(ulong player, int amount, bool print = true,
    string? reason = null) {
    return Task.FromResult(amount);
  }
}
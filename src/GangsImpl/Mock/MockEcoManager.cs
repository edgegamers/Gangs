using GangsAPI.Data;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace Mock;

public class MockEcoManager(IPlayerStatManager playerStats) : IEcoManager {
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

  private class MockCurrencyStat : IStat<int> {
    public bool Equals(IStat? other) {
      return other is not null && StatId == other.StatId;
    }

    public string StatId => "test_mock_currency";
    public string Name => "Test Mock Currency";
    public string? Description => null;

    public bool Equals(IStat<int>? other) { return Equals(other as IStat); }
    public int Value { get; set; } = 0;
  }
}
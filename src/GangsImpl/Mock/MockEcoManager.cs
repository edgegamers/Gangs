using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace Mock;

public class MockEcoManager(IPlayerStatManager playerStats) : IEcoManager {
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

  private readonly string statId = new MockCurrencyStat().StatId;

  public Task<bool> CanAfford(IGangPlayer player, int cost) {
    return Task.FromResult(getBalance(player).Result >= cost);
  }

  private async Task<int> getBalance(IGangPlayer player) {
    var (success, balance) =
      await playerStats.GetForPlayer<int>(player.Steam, statId);
    return success ? balance : 0;
  }

  public async Task<int> TryPurchase(IGangPlayer player, int cost,
    bool print = true, string? item = null) {
    await playerStats.SetForPlayer(player.Steam, statId,
      getBalance(player).Result - cost);
    return getBalance(player).Result;
  }

  public Task<int> Grant(ulong player, int amount, bool print = true,
    string? reason = null) {
    return Grant(player, amount, reason);
  }

  public Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null) {
    return Grant(gangId, amount, reason);
  }

  public async Task<int> Grant(ulong player, int amount, string reason = "") {
    var (success, balance) =
      await playerStats.GetForPlayer<int>(player, statId);
    if (success) {
      await playerStats.SetForPlayer(player, statId, balance + amount);
    }

    return balance + amount;
  }

  public Task<int> Grant(int gangId, int amount, string reason = "") {
    return Task.FromResult(0);
  }
}
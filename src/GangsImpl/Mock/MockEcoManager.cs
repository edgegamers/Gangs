using GangsAPI.Data;
using GangsAPI.Services;

namespace Mock;

public class MockEcoManager : IEcoManager {
  private readonly Dictionary<ulong, int> playerBals = new();
  private readonly Dictionary<int, int> gangBals = new();

  public Task<bool> CanAfford(PlayerWrapper player, int cost,
    bool excludeGangCredits = false) {
    var playerBal = playerBals.GetValueOrDefault(player.Steam);
    return Task.FromResult(playerBal >= cost);
  }

  public Task<int> GetBalance(PlayerWrapper player,
    bool excludeGangCredits = false) {
    return Task.FromResult(playerBals.GetValueOrDefault(player.Steam));
  }

  public Task<int> TryPurchase(PlayerWrapper player, int balanceDue,
    bool print = true, string? item = null, bool excludeGangCredits = false) {
    var bal       = GetBalance(player).Result;
    var resulting = bal - balanceDue;
    if (resulting < 0) return Task.FromResult(resulting);
    playerBals[player.Steam] = resulting;
    return Task.FromResult(resulting);
  }

  public Task<int> GetBalance(int gangId) {
    return Task.FromResult(gangBals.GetValueOrDefault(gangId));
  }

  public Task<int> Grant(PlayerWrapper player, int amount, bool print = true,
    string? reason = null) {
    var bal = GetBalance(player).Result;
    playerBals[player.Steam] = bal + amount;
    return Task.FromResult(bal + amount);
  }

  public Task<int> Grant(int gangId, int amount, bool print = true,
    string? reason = null) {
    var bal = gangBals.GetValueOrDefault(gangId);
    gangBals[gangId] = bal + amount;
    return Task.FromResult(bal + amount);
  }

  public Task<int> Grant(ulong player, int amount, bool print = true,
    string? reason = null) {
    var bal = playerBals.GetValueOrDefault(player);
    playerBals[player] = bal + amount;
    return Task.FromResult(bal + amount);
  }
}
﻿using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace Mock;

public class MockPlayerManager : IPlayerManager {
  private readonly Dictionary<ulong, IGangPlayer> players = new();

  public Task<IGangPlayer?> GetPlayer(ulong steamId) {
    players.TryGetValue(steamId, out var player);
    return Task.FromResult(player);
  }

  public async Task<IGangPlayer> CreatePlayer(ulong steamId,
    string? name = null) {
    var existing = await GetPlayer(steamId);
    if (existing != null) return existing;
    var player = new MockPlayer(steamId) { Name = name };
    players[steamId] = player;
    return player;
  }

  public Task<bool> UpdatePlayer(IGangPlayer player) {
    if (!players.ContainsKey(player.Steam)) return Task.FromResult(false);
    players[player.Steam] = player;
    return Task.FromResult(true);
  }

  public Task<bool> DeletePlayer(ulong steamId) {
    return Task.FromResult(players.Remove(steamId));
  }

  public void ClearCache() { players.Clear(); }

  public Task Load() { return Task.CompletedTask; }
}
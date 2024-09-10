using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Server;

namespace Mock;

public class MockServerProvider : IServerProvider {
  private readonly List<PlayerWrapper> players = [];

  public Task<IReadOnlyList<PlayerWrapper>> GetPlayers() {
    return Task.FromResult<IReadOnlyList<PlayerWrapper>>(players);
  }

  public Task AddPlayer(PlayerWrapper player) {
    players.Add(player);
    return Task.CompletedTask;
  }

  public Task<bool> RemovePlayer(PlayerWrapper player) {
    return Task.FromResult(players.Remove(player));
  }
}
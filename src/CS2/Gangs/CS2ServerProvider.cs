using CounterStrikeSharp.API;
using GangsAPI.Data;
using GangsAPI.Services.Server;

namespace GangsImpl;

public class CS2ServerProvider : IServerProvider {
  public Task AddPlayer(PlayerWrapper player) {
    throw new NotSupportedException();
  }

  public Task<bool> RemovePlayer(PlayerWrapper player) {
    throw new NotSupportedException();
  }

  async Task<IReadOnlyList<PlayerWrapper>> IServerProvider.GetPlayers() {
    var players = new List<PlayerWrapper>();

    await Server.NextFrameAsync(() => {
      players.AddRange(Utilities.GetPlayers()
       .Where(p => !p.IsBot)
       .Select(player => new PlayerWrapper(player)));
    });

    return players;
  }
}
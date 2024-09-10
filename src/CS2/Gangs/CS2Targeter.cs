using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using GangsAPI.Data;

namespace GangsImpl;

public class CS2Targeter : ITargeter {
  public async Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null) {
    var result = new List<PlayerWrapper>();
    await Server.NextFrameAsync(() => {
      var player = executor == null ?
        null :
        Utilities.GetPlayerFromSteamId(executor.Steam);
      result.AddRange(new Target(query).GetTarget(player)
       .Players.Select(p => new PlayerWrapper(p)));
    });

    return result;
  }

  public Task<PlayerWrapper?> GetSingleTarget(string query,
    out bool matchedMany, PlayerWrapper? executor = null) {
    var matches = GetTarget(query, executor).GetAwaiter().GetResult().ToList();
    matchedMany = matches.Count > 1;
    return Task.FromResult(matches.Count == 1 ? matches.First() : null);
  }
}
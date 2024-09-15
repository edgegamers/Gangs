using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Server;
using Microsoft.Extensions.Localization;

namespace GangsImpl;

public class Cs2PlayerTargeter : IPlayerTargeter {
  public async Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null) {
    var result = new List<PlayerWrapper>();
    await Server.NextFrameAsync(() => {
      var player = executor == null ?
        null :
        Utilities.GetPlayerFromSteamId(executor.Steam);
      if (query.All(char.IsDigit)) query = "#" + query;

      result.AddRange(new Target(query).GetTarget(player)
       .Players.Select(p => new PlayerWrapper(p)));
      if (localizer != null && result.Count == 0)
        executor?.PrintToChat(
          localizer.Get(MSG.GENERIC_PLAYER_NOT_FOUND, query));
    });

    return result;
  }

  public Task<PlayerWrapper?> GetSingleTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null) {
    bool matchedMany;
    var  matches = GetTarget(query, executor).GetAwaiter().GetResult().ToList();
    var  count   = matches.Count;
    matchedMany = count > 1;
    if (count != 1 && localizer != null)
      executor?.PrintToChat(localizer.Get(
        matchedMany ?
          MSG.GENERIC_PLAYER_FOUND_MULTIPLE :
          MSG.GENERIC_PLAYER_NOT_FOUND, query));

    return Task.FromResult(matches.Count == 1 ? matches.First() : null);
  }
}
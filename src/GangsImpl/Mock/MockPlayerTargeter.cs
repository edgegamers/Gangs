using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Server;
using Microsoft.Extensions.Localization;

namespace Mock;

public class MockPlayerTargeter(IServerProvider server) : IPlayerTargeter {
  public async Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null) {
    IEnumerable<PlayerWrapper> players = await server.GetPlayers();
    var result = query switch {
      "@all" => players,
      "@!me" => players.Where(player => player != executor),
      "@me" when executor != null => new List<PlayerWrapper> { executor },
      _ when query.All(char.IsDigit) => players.Where(player
        => player.Steam.ToString().Contains(query)),
      _ => players.Where(player
        => player.Name != null && player.Name.Contains(query)
        || player.Steam.ToString().Contains(query))
    };
    var playerWrappers = result.ToList();
    var empty          = playerWrappers.Count == 0;

    if (localizer != null && empty)
      executor?.PrintToChat(localizer.Get(MSG.GENERIC_PLAYER_NOT_FOUND, query));

    return playerWrappers;
  }

  public Task<PlayerWrapper?> GetSingleTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null) {
    var matches = GetTarget(query, executor).GetAwaiter().GetResult().ToList();
    var matchedMany = matches.Count > 1;
    if (localizer != null && matches.Count != 1)
      executor?.PrintToChat(localizer.Get(
        matchedMany ?
          MSG.GENERIC_PLAYER_FOUND_MULTIPLE :
          MSG.GENERIC_PLAYER_NOT_FOUND, query));

    return Task.FromResult(matches.Count == 1 ? matches.First() : null);
  }
}
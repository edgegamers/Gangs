using GangsAPI;
using GangsAPI.Data;

namespace Mock;

public class MockTargeter(IServerProvider server) : ITargeter {
  public async Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null) {
    IEnumerable<PlayerWrapper> players = await server.GetPlayers();
    return query switch {
      "@all" => players,
      "@!me" => players.Where(player => player != executor),
      "@me" when executor != null => new List<PlayerWrapper> { executor! },
      _ => players.Where(player
        => player.Name != null && player.Name.Contains(query)
        || player.Steam.ToString().Contains(query))
    };
  }

  public Task<PlayerWrapper?> GetSingleTarget(string query,
    out bool matchedMany, PlayerWrapper? executor = null) {
    var matches = GetTarget(query, executor).Result.ToList();
    matchedMany = matches.Count > 1;
    return Task.FromResult(matches.Count == 1 ? matches.First() : null);
  }
}
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Mock;

public class BasicGangTargeter(IServiceProvider provider)
  : IGangTargeter, IPluginBehavior {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerTargeter playerTargeter =
    provider.GetRequiredService<IPlayerTargeter>();

  public async Task<IGang?> FindGang(string query,
    PlayerWrapper? executor = null, bool print = true) {
    var existing = (await gangs.GetGangs()).ToList();

    var matches = existing
     .Where(g => g.Name.Equals(query, StringComparison.OrdinalIgnoreCase))
     .ToList();
    if (matches.Count == 1) return matches.First();

    matches = existing.Where(g => g.Name.Contains(query)).ToList();

    switch (matches.Count) {
      case 1:
        return matches.First();
      case 0:
        matches = existing.Where(g
            => g.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
         .ToList();
        break;
    }

    switch (matches.Count) {
      case 0:
        if (print)
          executor?.PrintToChat(localizer.Get(MSG.GANG_NOT_FOUND, query));
        break;
      case 1:
        return matches.First();
      default: {
        var closest = matches
         .OrderBy(g => CalcLevenshteinDistance(g.Name, query))
         .First();

        if (print)
          executor?.PrintToChat(localizer.Get(MSG.GANG_NOT_FOUND_CLOSEST, query,
            closest.Name));
        break;
      }
    }

    return null;
  }

  public async Task<IGang?> FindGangOrByMember(string query,
    PlayerWrapper? executor = null, bool print = true) {
    var gang = await FindGang(query, executor);
    if (gang != null) return gang;
    var player =
      await playerTargeter.GetSingleTarget(query, executor, localizer);
    if (player == null) {
      if (print)
        executor?.PrintToChat(localizer.Get(MSG.GANG_NOT_FOUND, query));
      return null;
    }

    gang = await gangs.GetGang(player.Steam);

    if (gang == null && print)
      executor?.PrintToChat(localizer.Get(MSG.GANG_NOT_FOUND, query));

    return gang;
  }

  private static int CalcLevenshteinDistance(string a, string b) {
    if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return 0;
    if (string.IsNullOrEmpty(a)) return b.Length;
    if (string.IsNullOrEmpty(b)) return a.Length;

    var lengthA   = a.Length;
    var lengthB   = b.Length;
    var distances = new int[lengthA + 1, lengthB + 1];
    for (var i = 0; i <= lengthA; distances[i, 0] = i++) { }

    for (var j = 0; j <= lengthB; distances[0, j] = j++) { }

    for (var i = 1; i <= lengthA; i++)
      for (var j = 1; j <= lengthB; j++) {
        var cost = b[j - 1] == a[i - 1] ? 0 : 1;
        distances[i, j] =
          Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
            distances[i - 1, j - 1] + cost);
      }

    return distances[lengthA, lengthB];
  }
}
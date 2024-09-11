using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class MembersCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly ITargeter targeter =
    provider.GetRequiredService<ITargeter>();

  public string Name => "members";

  public string[] Usage => ["", "<gang>"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount > 2) return CommandResult.PRINT_USAGE;
    int gangId;

    if (info.ArgCount == 1) {
      var gangPlayer = await players.GetPlayer(executor.Steam);
      if (gangPlayer == null) {
        info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO,
          "Gang Player not found"));
        return CommandResult.ERROR;
      }

      if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
        info.ReplySync(locale.Get(MSG.NOT_IN_GANG));
        return CommandResult.SUCCESS;
      }

      gangId = gangPlayer.GangId.Value;
      goto foundPlayer;
    }

    var memberPlayer = await targeter.GetSingleTarget(info[1], out _, executor);
    if (memberPlayer != null) {
      var gangPlayer = await players.GetPlayer(memberPlayer.Steam);
      if (gangPlayer is { GangId : not null, GangRank: not null }) {
        gangId = gangPlayer.GangId.Value;
        goto foundPlayer;
      }
    }
    // Regardless of if an online player was found, if we reached this spot,
    // we did not find a valid gang by player. So, look for a gang by name.

    var existingGangs = (await gangs.GetGangs()).ToList();

    var match = existingGangs.FirstOrDefault(g
      => g.Name.Equals(info[1], StringComparison.CurrentCultureIgnoreCase));

    if (match != null) {
      gangId = match.GangId;
      goto foundPlayer;
    }

    var matches = existingGangs.Where(g
        => g.Name.Contains(info[1], StringComparison.CurrentCultureIgnoreCase))
     .ToList();

    switch (matches.Count) {
      case 0:
        info.ReplySync(locale.Get(MSG.GANG_NOT_FOUND, info[1]));
        return CommandResult.SUCCESS;
      case 1:
        gangId = matches.First().GangId;
        goto foundPlayer;
      default:
        var closest = matches
         .OrderBy(g => CalcLevenshteinDistance(g.Name, info[1]))
         .First();

        info.ReplySync(locale.Get(MSG.GANG_NOT_FOUND_CLOSEST, closest.Name));
        return CommandResult.SUCCESS;
    }


  foundPlayer:
    var gang = await gangs.GetGang(gangId);
    if (gang == null) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO, "Gang not found"));
      return CommandResult.ERROR;
    }

    await menus.OpenMenu(executor, new MembersMenu(provider, gang));
    return CommandResult.SUCCESS;
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
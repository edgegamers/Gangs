using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class DemoteCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "demote";

  public override string[] Usage => ["<player>"];

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;

    var executorRank = await ranks.GetRank(player)
      ?? throw new GangException("You do not have a rank.");

    var (allowed, required) = await ranks.CheckRank(player, Perm.DEMOTE_OTHERS);

    if (!allowed) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.NO_PERMISSION;
    }

    var query = string.Join(' ', info.Args.Skip(1));

    Debug.Assert(player.GangId != null, "player.GangId != null");

    var gang = await gangs.GetGang(executor.Steam)
      ?? throw new GangNotFoundException(player);

    var target = await searchPlayer(gang, query);

    if (target == null) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_PLAYER_NOT_FOUND, query));
      return CommandResult.SUCCESS;
    }

    var targetRank = await ranks.GetRank(target)
      ?? throw new GangException("Target does not have a rank.");

    var lower = await ranks.GetLowerRank(gang.GangId, targetRank.Rank);
    // Trying to demote below the lowest rank, they need to kick instead
    if (lower == null) {
      info.ReplySync(Localizer.Get(MSG.RANK_DEMOTE_BELOW_LOWEST,
        target.Name ?? target.Steam.ToString()));
      return CommandResult.NO_PERMISSION;
    }

    if (targetRank.Rank <= executorRank.Rank) {
      // Can't demote someone with the same or higher rank
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, lower.Name));
      return CommandResult.NO_PERMISSION;
    }

    target.GangRank = lower.Rank;

    await players.UpdatePlayer(target);

    var gangChat = Provider.GetService<IGangChatPerk>();
    if (gangChat != null)
      await gangChat.SendGangChat(player, gang,
        Localizer.Get(MSG.RANK_DEMOTE_SUCCESS,
          target.Name ?? target.Steam.ToString(), lower.Name));
    return CommandResult.SUCCESS;
  }

  private async Task<IGangPlayer?> searchPlayer(IGang gang, string query) {
    var members = (await players.GetMembers(gang.GangId)).ToList();
    var player = members.FirstOrDefault(p
      => query.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
    if (player != null) return player;

    if (!ulong.TryParse(query, out var id)) return null;
    return members.FirstOrDefault(p => p.Steam == id);
  }
}
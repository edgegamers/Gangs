using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class DemoteCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "demote";

  public override string[] Usage => ["<player>"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;

    var executorRank = await Ranks.GetRank(player)
      ?? throw new GangException("You do not have a rank.");

    var (allowed, required) = await Ranks.CheckRank(player, Perm.DEMOTE_OTHERS);

    if (!allowed) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.NO_PERMISSION;
    }

    var query = string.Join(' ', info.Args.Skip(1));

    Debug.Assert(player.GangId != null, "player.GangId != null");

    var gang = await Gangs.GetGang(executor.Steam)
      ?? throw new GangNotFoundException(player);

    var target = await Players.SearchPlayer(gang, query);

    if (target == null) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_PLAYER_NOT_FOUND, query));
      return CommandResult.SUCCESS;
    }

    var targetRank = await Ranks.GetRank(target)
      ?? throw new GangException("Target does not have a rank.");

    var lower  = await Ranks.GetLowerRank(gang.GangId, targetRank.Rank);
    var higher = await Ranks.GetHigherRank(gang.GangId, targetRank.Rank);

    // Trying to demote below the lowest rank, they need to kick instead
    if (lower == null) {
      info.ReplySync(Localizer.Get(MSG.RANK_DEMOTE_BELOW_LOWEST,
        target.Name ?? target.Steam.ToString()));
      return CommandResult.NO_PERMISSION;
    }

    if (targetRank.Rank <= executorRank.Rank) {
      // Can't demote someone with the same or higher rank
      if (higher == null) {
        // No higher rank, can't demote
        info.ReplySync(Localizer.Get(MSG.RANK_CANNOT_OWNER, "demote"));
        return CommandResult.NO_PERMISSION;
      }

      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, higher.Name));
      return CommandResult.NO_PERMISSION;
    }

    target.GangRank = lower.Rank;

    await Players.UpdatePlayer(target);

    var gangChat = Provider.GetService<IGangChatPerk>();
    if (gangChat != null)
      await gangChat.SendGangChat(player, gang,
        Localizer.Get(MSG.RANK_DEMOTE_SUCCESS,
          target.Name ?? target.Steam.ToString(), lower.Name));
    return CommandResult.SUCCESS;
  }
}
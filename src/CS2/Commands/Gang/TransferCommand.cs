using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Permissions;

namespace Commands.Gang;

public class TransferCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "transfer";
  public override string[] Usage => ["<player>"];

  public override string Description
    => "Transfer leadership of the gang to another player";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;
    var rank = player.GangRank;

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var (success, required) = await Ranks.CheckRank(player, Perm.OWNER);
    if (required == null)
      throw new SufficientRankNotFoundException(player.GangId.Value,
        Perm.OWNER);

    if (!success) {
      info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.SUCCESS;
    }

    if (rank != 0)
      throw new GangException("Passed rank check but not numerical check");

    Debug.Assert(player.GangRank != null, "player.GangRank != null");
    var immediatelyLower =
      await Ranks.GetLowerRank(player.GangId.Value, player.GangRank.Value);

    if (immediatelyLower == null) {
      info.ReplySync(Locale.Get(MSG.GENERIC_ERROR_INFO,
        "Could not find a lower rank"));
      return CommandResult.SUCCESS;
    }

    var query  = string.Join(' ', info.Args.Skip(1));
    var target = await Players.FindPlayerInGang(player.GangId.Value, query);
    if (target == null) {
      info.ReplySync(Locale.Get(MSG.GENERIC_PLAYER_NOT_FOUND, query));
      return CommandResult.SUCCESS;
    }

    var targetRank = await Ranks.GetRank(target)
      ?? throw new GangException("Target does not have a rank.");

    if (targetRank != immediatelyLower) {
      info.ReplySync(Locale.Get(MSG.COMMAND_TRANSFER_SUBORDINATE,
        immediatelyLower.Name));
      return CommandResult.SUCCESS;
    }

    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    target.GangRank = 0;
    await Players.UpdatePlayer(target);

    player.GangRank = immediatelyLower.Rank;
    await Players.UpdatePlayer(player);

    if (GangChat != null)
      await GangChat.SendGangChat(gang,
        Locale.Get(MSG.GANG_TRANSFERRED,
          executor.Name ?? executor.Steam.ToString(),
          target.Name ?? target.Steam.ToString()));

    return CommandResult.SUCCESS;
  }
}
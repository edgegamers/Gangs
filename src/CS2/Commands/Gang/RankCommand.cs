using System.Diagnostics;
using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class RankCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly IGangChatPerk gangChat =
    provider.GetRequiredService<IGangChatPerk>();

  public override string Name => "rank";

  public override string[] Usage
    => ["", "create <rank> <name>", "delete <rank>", "rename <rank> <name>"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player);
    if (info.ArgCount == 1) {
      await Menus.OpenMenu(executor, new RankMenu(Provider, gang));
      return CommandResult.SUCCESS;
    }

    if (info.ArgCount < 3) return CommandResult.PRINT_USAGE;

    var executorRank = await Ranks.GetRank(player)
      ?? throw new RankNotFoundException(player);

    if (!int.TryParse(info[2], out var targetRank)) {
      info.ReplySync(Locale.Get(MSG.COMMAND_INVALID_PARAM, info[2],
        "an integer"));
      return CommandResult.INVALID_ARGS;
    }

    if (executorRank.Rank != 0 && executorRank.Rank >= targetRank
      || !executorRank.Permissions.HasFlag(Perm.MANAGE_RANKS)) {
      var higher = await Ranks.GetHigherRank(gang.GangId, targetRank)
        ?? throw new RankNotFoundException(gang.GangId, targetRank);
      info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, higher.Name));
      return CommandResult.NO_PERMISSION;
    }

    switch (info[1].ToLower()) {
      case "create":
        if (info.ArgCount < 4) return CommandResult.PRINT_USAGE;
        return await handleCreate(player, gang, targetRank, info);
      case "delete":
        return await handleDelete(player, gang, targetRank, info);
      case "rename":
        if (info.ArgCount < 4) return CommandResult.PRINT_USAGE;
        return await handleRename(player, gang, targetRank, info);
      default:
        return CommandResult.PRINT_USAGE;
    }
  }

  // rank create <rank> <name>
  private async Task<CommandResult> handleCreate(IGangPlayer player, IGang gang,
    int rank, CommandInfoWrapper info) {
    var name     = string.Join(' ', info.Args.Skip(3));
    var existing = await Ranks.GetRank(gang.GangId, rank);
    if (existing != null) {
      info.ReplySync(Locale.Get(MSG.COMMAND_RANK_EXISTS, existing.Name));
      return CommandResult.ERROR;
    }

    var newRank = await Ranks.CreateRank(gang.GangId, name, rank, Perm.NONE);
    if (newRank == null) {
      info.ReplySync(Locale.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    await gangChat.SendGangChat(player, gang,
      Locale.Get(MSG.COMMAND_RANK_CREATED, newRank.Name));
    return CommandResult.SUCCESS;
  }

  private async Task<CommandResult> handleDelete(IGangPlayer player, IGang gang,
    int rank, CommandInfoWrapper info) {
    var target = await Ranks.GetRank(gang.GangId, rank)
      ?? throw new RankNotFoundException(gang.GangId, rank);
    if (target.Rank == 0) {
      info.ReplySync(Locale.Get(MSG.COMMAND_RANK_CANNOT_DELETE,
        target.Name));
      return CommandResult.ERROR;
    }

    var assigned = (await Players.GetMembers(gang))
     .Where(p => p.GangRank == rank)
     .ToList();

    if (assigned.Count != 0) {
      info.ReplySync(Locale.Get(MSG.COMMAND_RANK_CANNOT_DELETE,
        target.Name));
      return CommandResult.SUCCESS;
    }

    var result = await Ranks.DeleteRank(gang.GangId, target,
      IRankManager.DeleteStrat.DEMOTE_FAIL);
    if (!result) {
      info.ReplySync(Locale.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    await gangChat.SendGangChat(player, gang,
      Locale.Get(MSG.COMMAND_RANK_DELETED, target.Name));
    return CommandResult.SUCCESS;
  }

  private async Task<CommandResult> handleRename(IGangPlayer player, IGang gang,
    int rank, CommandInfoWrapper info) {
    var target = await Ranks.GetRank(gang.GangId, rank)
      ?? throw new RankNotFoundException(gang.GangId, rank);

    var name    = string.Join(' ', info.Args.Skip(3));
    var oldName = target.Name;
    target.Name = name;

    var result = await Ranks.UpdateRank(gang.GangId, target);
    if (!result) {
      info.ReplySync(Locale.Get(MSG.GENERIC_ERROR));
      return CommandResult.ERROR;
    }

    await gangChat.SendGangChat(player, gang,
      Locale.Get(MSG.COMMAND_RANK_RENAMED, oldName, target.Name));
    return CommandResult.SUCCESS;
  }
}
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using Stats;

namespace Commands.Gang;

public class InviteCommand(IGangManager gangs, IPlayerManager playerMgr,
  IRankManager rankMgr, IGangStatManager gangStatMgr,
  IStringLocalizer localizer) : ICommand {
  private readonly string statId = new InvitationStat().StatId;

  public string Name => "invite";

  public string? Description => "Invites a player to the gang";

  public string[] Usage => ["invite [player/steam]"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    var gangPlayer = await playerMgr.GetPlayer(executor.Steam);

    if (gangPlayer == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Gang Player not found"));
      return CommandResult.ERROR;
    }

    if (gangPlayer.GangId == null || gangPlayer.GangRank == null) {
      info.ReplySync(localizer.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var perms = await rankMgr.GetRank(gangPlayer.GangId.Value,
      gangPlayer.GangRank.Value);

    if (perms == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO, "Rank not found"));
      return CommandResult.ERROR;
    }

    if (!perms.Permissions.HasFlag(Perm.INVITE_OTHERS)) {
      var required =
        await rankMgr.GetRankNeeded(gangPlayer.GangId.Value,
          Perm.INVITE_OTHERS);
      info.ReplySync(localizer.Get(MSG.GENERIC_NOPERM_RANK, required));
      return CommandResult.NO_PERMISSION;
    }

    var (success, invites) =
      await gangStatMgr.GetForGang<InvitationData>(gangPlayer.GangId.Value,
        statId);
    if (!success || invites == null) invites = new InvitationData();

    var inviteList = invites.GetEntries();

    if (inviteList.Count >= invites.MaxAmo) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_MAXINVITES,
        invites.MaxAmo));
      return CommandResult.ERROR;
    }

    ulong? steam = null;
    if (info[1].All(char.IsDigit))
      steam = ulong.Parse(info[1]);
    else {
      await Server.NextFrameAsync(() => {
        var players = new Target(info[1]).GetTarget(null).Players;
        if (players.Count != 1) {
          info.ReplySync(localizer.Get(
            players.Count > 1 ?
              MSG.GENERIC_PLAYER_FOUND_MULTIPLE :
              MSG.GENERIC_PLAYER_NOT_FOUND, info[1]));
        }

        steam = players[0].SteamID;
      });
    }

    if (steam == null) return CommandResult.INVALID_ARGS;

    var offlinePlayer = await playerMgr.GetPlayer(steam.Value, false);
    if (offlinePlayer == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_STEAM_NOT_FOUND, steam));
      return CommandResult.ERROR;
    }

    if (invites.GetInvitedSteams().Contains(steam.Value)) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED,
        offlinePlayer?.Name ?? offlinePlayer!.Steam.ToString()));
      return CommandResult.SUCCESS;
    }

    if (offlinePlayer.GangId != null) {
      info.ReplySync(localizer.Get(
        offlinePlayer.GangId == gangPlayer.GangId ?
          MSG.COMMAND_INVITE_IN_YOUR_GANG :
          MSG.ALREADY_IN_GANG,
        offlinePlayer?.Name ?? offlinePlayer!.Steam.ToString()));
    }

    var gangName = (await gangs.GetGang(gangPlayer.GangId.Value))?.Name;

    if (gangName == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Gang name not found"));
      return CommandResult.ERROR;
    }

    invites.AddInvitation(executor.Steam, steam.Value);
    await gangStatMgr.SetForGang(gangPlayer.GangId.Value, statId, invites);


    info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_SUCCESS,
      offlinePlayer?.Name ?? offlinePlayer!.Steam.ToString(), gangName));
    return CommandResult.SUCCESS;
  }
}
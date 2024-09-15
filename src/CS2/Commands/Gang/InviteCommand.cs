using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat.Gang;
using Stats.Stat.Player;

namespace Commands.Gang;

public class InviteCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly string gangInviteId = new InvitationStat().StatId;
  private readonly string playerPendingId = new PendingInvitations().StatId;

  private readonly IPlayerTargeter playerTargeter =
    provider.GetRequiredService<IPlayerTargeter>();

  public override string Name => "invite";

  public override string Description => "Invites a player to the gang";

  public override string[] Usage => ["[player/steam]", "cancel [player/steam]"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount is 1 or > 3) return CommandResult.PRINT_USAGE;

    Debug.Assert(player.GangId != null, "player.GangId != null");

    var perms = await ranks.GetRank(player)
      ?? throw new SufficientRankNotFoundException(player.GangId.Value,
        Perm.INVITE_OTHERS);

    if (!perms.Permissions.HasFlag(Perm.INVITE_OTHERS)) {
      return await HandleNoPermission(player, info);
    }

    var (success, invites) =
      await gangStats.GetForGang<InvitationData>(player.GangId.Value,
        gangInviteId);
    if (!success || invites == null) invites = new InvitationData();

    if (info[1].Equals("cancel", StringComparison.OrdinalIgnoreCase)) {
      return await HandleCancelInvite(info, player, invites);
    }

    return await HandleInvite(executor, player, info, invites);
  }

  private async Task<CommandResult> HandleNoPermission(IGangPlayer player,
    CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var required =
      await ranks.GetRankNeeded(player.GangId.Value, Perm.INVITE_OTHERS);
    info.ReplySync(localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
    return CommandResult.NO_PERMISSION;
  }

  private async Task<CommandResult> HandleCancelInvite(CommandInfoWrapper info,
    IGangPlayer player, InvitationData invites) {
    if (info.ArgCount != 3) return CommandResult.PRINT_USAGE;

    var    query         = info[2];
    var    invitedSteams = invites.GetInvitedSteams();
    ulong? steamId;

    if (!query.All(char.IsDigit))
      steamId = await RemoveInviteByName(info, player, invites, query,
        invitedSteams);
    else {
      steamId = await RemoveInviteBySteamId(info, player, invites,
        ulong.Parse(query), query);
    }

    if (steamId == null) return CommandResult.SUCCESS;
    await CancelPendingInvitation(player, steamId.Value);
    return CommandResult.SUCCESS;
  }

  private async Task<ulong?> RemoveInviteBySteamId(CommandInfoWrapper info,
    IGangPlayer player, InvitationData invites, ulong steamId, string query) {
    if (invites.RemoveInvitation(steamId)) {
      Debug.Assert(player.GangId != null, "player.GangId != null");
      await gangStats.SetForGang(player.GangId.Value, gangInviteId, invites);
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_CANCELED, query));
    } else {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_NOTFOUND, query));
    }

    return steamId;
  }

  private async Task<ulong?> RemoveInviteByName(CommandInfoWrapper info,
    IGangPlayer player, InvitationData invites, string query,
    List<ulong> invitedSteams) {
    var invitedNames = await GetMatchingInvitedNames(query, invitedSteams);

    switch (invitedNames.Count) {
      case 0:
        info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_NOTFOUND, query));
        return null;
      case > 1:
        info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_FOUNDMULTIPLE, query));
        return null;
    }

    var (steamId, name) = invitedNames.First();
    if (invites.RemoveInvitation(steamId)) {
      Debug.Assert(player.GangId != null, "player.GangId != null");
      await gangStats.SetForGang(player.GangId.Value, gangInviteId, invites);
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_CANCELED, name));
    } else
      throw new GangException(
        $"failed to remove invite which we found via {query}");


    return steamId;
  }

  private async Task<List<(ulong, string)>> GetMatchingInvitedNames(
    string query, List<ulong> invitedSteams) {
    var invitedNames = new List<(ulong, string)>();

    foreach (var steamId in invitedSteams) {
      var invitingPlayer = await players.GetPlayer(steamId, false);
      if (invitingPlayer?.Name != null
        && invitingPlayer.Name.Contains(query,
          StringComparison.OrdinalIgnoreCase)) {
        invitedNames.Add((steamId, invitingPlayer.Name));
      }
    }

    return invitedNames;
  }

  private async Task<CommandResult> HandleInvite(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info, InvitationData invites) {
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    if (invites.GetEntries().Count >= invites.MaxAmo) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_MAXINVITES,
        invites.MaxAmo));
      return CommandResult.ERROR;
    }

    var steam = await GetSteamId(info, executor);
    if (steam == null) return CommandResult.INVALID_ARGS;

    return await ProcessInvite(executor, player, info, invites, steam.Value);
  }

  private async Task<ulong?> GetSteamId(CommandInfoWrapper info,
    PlayerWrapper executor) {
    if (info[1].All(char.IsDigit)) { return ulong.Parse(info[1]); }

    return (await playerTargeter.GetSingleTarget(info[1], out _, executor,
      localizer))?.Steam;
  }

  private async Task<CommandResult> ProcessInvite(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info, InvitationData invites,
    ulong steam) {
    var offlinePlayer = await players.GetPlayer(steam, false);
    if (offlinePlayer == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_STEAM_NOT_FOUND, steam));
      return CommandResult.SUCCESS;
    }

    if (invites.GetInvitedSteams().Contains(steam)) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_ALREADY_INVITED,
        offlinePlayer.Name ?? offlinePlayer.Steam.ToString()));
      return CommandResult.SUCCESS;
    }

    if (offlinePlayer.GangId != null) {
      var msg = offlinePlayer.GangId == player.GangId ?
        MSG.COMMAND_INVITE_IN_YOUR_GANG :
        MSG.COMMAND_INVITE_ALREADY_IN_GANG;
      info.ReplySync(localizer.Get(msg,
        offlinePlayer.Name ?? offlinePlayer.Steam.ToString()));
      return CommandResult.SUCCESS;
    }

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gangName = (await gangs.GetGang(player.GangId.Value))?.Name;
    if (gangName == null) {
      info.ReplySync(localizer.Get(MSG.GENERIC_ERROR_INFO,
        "Gang name not found"));
      return CommandResult.ERROR;
    }

    invites.AddInvitation(executor.Steam, steam);
    await gangStats.SetForGang(player.GangId.Value, gangInviteId, invites);

    info.ReplySync(localizer.Get(MSG.COMMAND_INVITE_SUCCESS,
      offlinePlayer.Name ?? offlinePlayer.Steam.ToString(), gangName));

    return await AddPendingInvitation(player, offlinePlayer.Steam);
  }

  private async Task<CommandResult> AddPendingInvitation(IGangPlayer player,
    ulong steam) {
    var (fetchedPending, pending) =
      await playerStats.GetForPlayer<PendingInvitationData>(steam,
        playerPendingId);
    if (!fetchedPending || pending == null) {
      pending = new PendingInvitationData();
    }

    Debug.Assert(player.GangId != null, "player.GangId != null");
    pending.AddInvitation(player.GangId.Value);
    await playerStats.SetForPlayer(steam, playerPendingId, pending);

    return CommandResult.SUCCESS;
  }

  private async Task CancelPendingInvitation(IGangPlayer player, ulong steam) {
    var (fetchedPending, pending) =
      await playerStats.GetForPlayer<PendingInvitationData>(steam,
        playerPendingId);
    if (!fetchedPending || pending == null) return;

    Debug.Assert(player.GangId != null, "player.GangId != null");
    pending.RemoveInvitation(player.GangId.Value);
    await playerStats.SetForPlayer(steam, playerPendingId, pending);
  }
}
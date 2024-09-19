using System.Text.Json.Serialization;
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
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Stats.Stat.Gang;

namespace Commands.Gang;

public class JoinCommand(IServiceProvider provider) : ICommand {
  private readonly ICapacityPerk? capacityPerk =
    provider.GetService<ICapacityPerk>();

  private readonly IGangChatPerk? chatPerk =
    provider.GetService<IGangChatPerk>();

  private readonly string doorPolicyId = new DoorPolicyStat().StatId;

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IGangTargeter gangTargeter =
    provider.GetRequiredService<IGangTargeter>();

  private readonly string invitationId = new InvitationStat().StatId;

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public string Name => "join";
  public string[] Usage => ["[gang]"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    if (info.ArgCount == 1) return CommandResult.PRINT_USAGE;

    var gangPlayer = await players.GetPlayer(executor.Steam)
      ?? throw new PlayerNotFoundException(executor.Steam);

    if (gangPlayer.GangId != null) {
      info.ReplySync(localizer.Get(MSG.ALREADY_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var name = string.Join(' ', info.Args.Skip(1));

    var gang = await gangTargeter.FindGang(name, executor);

    if (gang == null) return CommandResult.SUCCESS;

    if (capacityPerk != null) {
      var capacity = await capacityPerk.GetCapacity(gang);
      var members  = (await players.GetMembers(gang)).ToList();

      if (members.Count >= capacity) {
        info.ReplySync(localizer.Get(MSG.GANG_FULL, gang.Name));
        return CommandResult.SUCCESS;
      }
    }

    var (fetchedPolicy, policy) =
      await gangStats.GetForGang<DoorPolicy>(gang, doorPolicyId);

    if (!fetchedPolicy) policy = DoorPolicy.REQUEST_ONLY;

    if (policy == DoorPolicy.OPEN) {
      await joinGang(gangPlayer, gang);
      return CommandResult.SUCCESS;
    }

    var (fetchedInvites, inviteData) =
      await gangStats.GetForGang<InvitationData>(gang, invitationId);

    if (!fetchedInvites || inviteData == null)
      inviteData = new InvitationData();

    switch (policy) {
      case DoorPolicy.INVITE_ONLY:
        if (!inviteData.GetInvitedSteams().Contains(gangPlayer.Steam)) {
          info.ReplySync(localizer.Get(MSG.GANG_POLICY_INVITE_ONLY));
          return CommandResult.SUCCESS;
        }

        info.ReplySync(
          localizer.Get(MSG.GANG_POLICY_INVITE_ACCEPTED, gang.Name));
        await joinGang(gangPlayer, gang);
        inviteData.RemoveInvitation(gangPlayer.Steam);
        await gangStats.SetForGang(gang, invitationId, inviteData);
        break;
      case DoorPolicy.REQUEST_ONLY:
        if (inviteData.GetRequestedSteams().Contains(gangPlayer.Steam)) {
          info.ReplySync(localizer.Get(MSG.GANG_POLICY_REQUEST_ALREADY,
            gang.Name));
          return CommandResult.SUCCESS;
        }

        inviteData.AddRequest(gangPlayer.Steam);
        await gangStats.SetForGang(gang, invitationId, inviteData);
        info.ReplySync(localizer.Get(MSG.GANG_POLICY_REQUEST_SENT, gang.Name));
        break;
    }

    return CommandResult.SUCCESS;
  }

  private async Task joinGang(IGangPlayer player, IGang gang) {
    player.GangId = gang.GangId;
    var lowest = await ranks.GetJoinRank(gang)
      ?? throw new SufficientRankNotFoundException(gang.GangId, Perm.NONE);
    player.GangRank = lowest.Rank;

    await players.UpdatePlayer(player);
    if (chatPerk != null) await chatPerk.SendGangChat(player, gang, "joined.");
  }
}
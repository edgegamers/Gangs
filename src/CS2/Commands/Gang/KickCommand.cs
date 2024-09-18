using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class KickCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public override string Name => "kick";

  public override string[] Usage => ["<player>"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;

    var executorRank = await ranks.GetRank(player)
      ?? throw new GangException("You do not have a rank.");

    var (allowed, required) = await ranks.CheckRank(player, Perm.KICK_OTHERS);

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

    var higherRank = await ranks.GetHigherRank(gang.GangId, targetRank.Rank);

    if (higherRank == null) {
      info.ReplySync(Localizer.Get(MSG.RANK_CANNOT_OWNER, "kick yourself"));
      return CommandResult.NO_PERMISSION;
    }

    if (targetRank.Rank <= executorRank.Rank) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, higherRank.Name));
      return CommandResult.NO_PERMISSION;
    }

    target.GangId   = null;
    target.GangRank = null;

    await players.UpdatePlayer(target);

    var gangChat = Provider.GetService<IGangChatPerk>();

    if (gangChat != null)
      await gangChat.SendGangChat(player, gang,
        Localizer.Get(MSG.COMMAND_GANG_KICKED,
          target.Name ?? target.Steam.ToString()));
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